using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Collections;

public class FaceFXController : MonoBehaviour
{
    public bool IsReady;

    public bool UseReferencePoseFromFBX = false;

    float ScaleFactor = 1.0f;

    /*
        The facefx controller is an immediate child of the player GameObject.  It holds per-animation information like the audio start time 
        and curves for bone poses.  The audio start time is stored in the localPosition.x property of the facefx controller.  The controller 
        has one child per bone pose, and the bone pose curves are stored in the localPosition.x property of the children.  The bone poses 
        themselves are stored as Unity animations with the "facefx" prefix.  They drive the skeleton and are blended additively.  This 
        object is created by the FaceFXImportXMLActor Editor script.
     */
    GameObject goFacefx;

    /*
         The audio_clip to play associated with animation_name.  The audio start time is recorded for each animation in localPosition.x property
         of the facefx_controller for that animation.  Once playing, the audio determines the evaluation time of animation_name to prevent the 
         audio from getting out of synch with the animation.
     */
    public AudioClip audio_clip;

    /*
         The name of the animation.  This animation should not move bones directly, 
         but rather drive the children of the facefx_controller.
     */
    string animation_name;

    /*
        0 - Not playing / Ready to play.
        1 - playing animation prior to audio.
        2 - playing with audio.
        3 - playing after audio ends.
        4 - Blending out.
    */
    int play_state;

    // Cache the starting time of the audio. This is stored in the localPosition.x property of the facefx_controller for each animation.
    float audio_start_time;

    // We don't want to evaluate animations beyond their end time, so we use this to cache the animation evaluation time.
    float anim_eval_time;

    // An inverse_hermite curve is computed and cached because of the way bone poses need to be driven within unity's animation system.
    static AnimationCurve inverse_hermite;

    // A switch to tell anyone who cares that we have finished playing an animation and are ready for the next one.
    static bool switch_anim;

    // A class storing a bone transform.  The constructor takes the contents of a <bone> xml body (Space-seperated position rotation scale values).
    class BoneTransform
    {
        public float[] Values = new float[10];

        // Constructs a BoneTransform from a space-separated string (originating from an XML file)

        public BoneTransform(string aValue)
        {
            string[] StringValues = aValue.Split();

            if (StringValues.Length != 10)
            {
                Debug.Log("Error in XML.  A reference boen has only this many values:" + Values.Length);
                Debug.Log(aValue);
            }
            else
            {
                // Position (x, y, z)
                // @todo - Figure out why Pos.x and Rot.x values need to be negated.
				
                Values[0] = -float.Parse(StringValues[0]);
                Values[1] = float.Parse(StringValues[1]);
                Values[2] = float.Parse(StringValues[2]);

                // Rotation (x, y, z, w but in the XML file it is stored as w,x,y,z)  
                Values[3] = -float.Parse(StringValues[4]);
                Values[4] = float.Parse(StringValues[5]);
                Values[5] = float.Parse(StringValues[6]);
                Values[6] = float.Parse(StringValues[3]);

                // Scale (x, y, z)
                Values[7] = float.Parse(StringValues[7]);
                Values[8] = float.Parse(StringValues[8]);
                Values[9] = float.Parse(StringValues[9]);
            }
        }

        public BoneTransform(GameObject t)
        {
            Values[0] = t.transform.localPosition.x;
            Values[1] = t.transform.localPosition.y;
            Values[2] = t.transform.localPosition.z;
            Values[3] = t.transform.localRotation.x;
            Values[4] = t.transform.localRotation.y;
            Values[5] = t.transform.localRotation.z;
            Values[6] = t.transform.localRotation.w;
            Values[7] = t.transform.localScale.x;
            Values[8] = t.transform.localScale.y;
            Values[9] = t.transform.localScale.z;
        }

        public BoneTransform(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            Values[0] = pos.x;
            Values[1] = pos.y;
            Values[2] = pos.z;
            Values[3] = rot.x;
            Values[4] = rot.y;
            Values[5] = rot.z;
            Values[6] = rot.w;
            Values[7] = scale.x;
            Values[8] = scale.y;
            Values[9] = scale.z;
        }

        void Print()
        {
            Debug.Log("( " + Values[0] + ", " + Values[1] + ", " + Values[2] + ") (" + Values[3] + ", " + Values[4] + ", " + Values[5] + ", " + Values[6] + ") (" + Values[7] + ", " + Values[8] + ", " + Values[9] + ")");
        }

        public Vector3 GetPos()
        {
            return new Vector3(Values[0], Values[1], Values[2]);
        }

        public Quaternion GetRot()
        {
            return new Quaternion(Values[3], Values[4], Values[5], Values[6]);
        }

        public Vector3 GetScale()
        {
            return new Vector3(Values[7], Values[8], Values[9]);
        }
    }

    // A class to help manage adding keys to curves and curves to clips.
    class AnimClipHelper
    {
        AnimationCurve curvePosX;
        AnimationCurve curvePosY;
        AnimationCurve curvePosZ;
        AnimationCurve curveRotX;
        AnimationCurve curveRotY;
        AnimationCurve curveRotZ;
        AnimationCurve curveRotW;
        AnimationCurve curveScaleX;
        AnimationCurve curveScaleY;
        AnimationCurve curveScaleZ;
        public AnimationClip animclip;

        public AnimClipHelper()
        {
            animclip = new AnimationClip();
        }

        public void PreAddKeys()
        {
            curvePosX = new AnimationCurve();
            curvePosY = new AnimationCurve();
            curvePosZ = new AnimationCurve();
            curveRotX = new AnimationCurve();
            curveRotY = new AnimationCurve();
            curveRotZ = new AnimationCurve();
            curveRotW = new AnimationCurve();
            curveScaleX = new AnimationCurve();
            curveScaleY = new AnimationCurve();
            curveScaleZ = new AnimationCurve();
        }

        public void AddKeys(float t, BoneTransform values)
        {

            // Position x,y,z
            curvePosX.AddKey(new Keyframe(t, values.Values[0]));
            curvePosY.AddKey(new Keyframe(t, values.Values[1]));
            curvePosZ.AddKey(new Keyframe(t, values.Values[2]));

            // Rotation x,y,z,w
            curveRotX.AddKey(new Keyframe(t, values.Values[3]));
            curveRotY.AddKey(new Keyframe(t, values.Values[4]));
            curveRotZ.AddKey(new Keyframe(t, values.Values[5]));
            curveRotW.AddKey(new Keyframe(t, values.Values[6]));

            // Scale x,y,z
            curveScaleX.AddKey(new Keyframe(t, values.Values[7]));
            curveScaleY.AddKey(new Keyframe(t, values.Values[8]));
            curveScaleZ.AddKey(new Keyframe(t, values.Values[9]));
        }

        public void PostAddKeys(string objectRelativePath)
        {
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localPosition.x", curvePosX);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localPosition.y", curvePosY);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localPosition.z", curvePosZ);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localRotation.x", curveRotX);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localRotation.y", curveRotY);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localRotation.z", curveRotZ);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localRotation.w", curveRotW);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localScale.x", curveScaleX);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localScale.y", curveScaleY);
            animclip.SetCurve(objectRelativePath, typeof(Transform), "localScale.z", curveScaleZ);
        }
    }

    void InitializeFaceFXController(GameObject ffxController)
    {
        if (ffxController == null)
        {
            Debug.LogError("Can not initialize null FaceFX controller.");
        }
        else
        {
            goFacefx = ffxController;
            foreach (Transform child in goFacefx.transform)
            {
                AnimationState bonePoseAnim = GetComponent<Animation>()[child.name];
                if (bonePoseAnim != null)
                {
                    // Keep bone pose animations in their own layer with addative blending
                    // and ClampForever wrapping.  Enable them and set the weight to 1.
                    // We are then prepared to manually adjust the "time" of the animation 
                    // in the Update function to control the amount the bone pose is blended in.
                    bonePoseAnim.layer = 10;
                    bonePoseAnim.blendMode = AnimationBlendMode.Additive;
                    bonePoseAnim.wrapMode = WrapMode.ClampForever;
                    bonePoseAnim.enabled = true;
                    bonePoseAnim.weight = 1;
                }
            }
            if (GetComponent<Animation>() == null)
            {
                Debug.Log("Warning.  Animation component must be attached to " + name + " character for animations to play!");
            }
            else
            {

                // The loop anim is created from the XML import.  We need a non-additive animation to play,
                // And it just has the reference pose.
                AnimationState loopAnim = GetComponent<Animation>()["facefx_loop_anim"];
                if (loopAnim != null)
                {
                    loopAnim.layer = -1;
                    loopAnim.wrapMode = WrapMode.ClampForever;
                    GetComponent<Animation>().Play("facefx_loop_anim");
                }
                else
                {
                    Debug.Log("No facefx_loop_anim animation found for " + name + ".  The facefx_controller is likely corrupt and should be reimported.");
                }
            }

            foreach (Transform child in goFacefx.transform)
            {
                AnimationState anim = GetComponent<Animation>()[child.name];
                if (anim != null)
                {
                    // This prevents bones from shaking.
                    anim.normalizedSpeed = 0;
                }
            }
        }
    }

    void ImportXMLAnimations(string xmltext)
    {
        XmlDocument doc_reader = new XmlDocument();
        doc_reader.Load(new StringReader(xmltext));
        Transform facefx_controller = transform.Find("facefx_controller");

        if (facefx_controller != null)
        {
            XmlNodeList animNodeList = doc_reader.SelectNodes("/actor/animation_groups/animation_group/animation/curves");
            for (int i = 0; i < animNodeList.Count; ++i)
            {
                float lastKeyTime = 0;
                float firstKeyTime = 0;

                var curvesNode = animNodeList.Item(i);
                string animName = curvesNode.ParentNode.Attributes["name"].Value;
                string animGroupName = curvesNode.ParentNode.ParentNode.Attributes["name"].Value;

                AnimClipHelper controllerAnimHelper = new AnimClipHelper();

                List<AnimationCurve> curveArray = new List<AnimationCurve>();
                List<string> relativePathArray = new List<string>();
                for (int j = 0; j < curvesNode.ChildNodes.Count; ++j)
                {
                    var curveFirstKeyNode = curvesNode.ChildNodes.Item(j);
                    int first_keytime_end = curveFirstKeyNode.InnerText.IndexOf(" ");
                    if (first_keytime_end > -1)
                    {
                        string first_keytime_string = curveFirstKeyNode.InnerText.Substring(0, first_keytime_end);
                        float first_keytime = float.Parse(first_keytime_string);
                        if (first_keytime < firstKeyTime)
                        {
                            firstKeyTime = first_keytime;
                        }
                    }
                }
                if (firstKeyTime > 0)
                {
                    firstKeyTime = 0;
                }
                for (int j = 0; j < curvesNode.ChildNodes.Count; ++j)
                {
                    XmlNode curveNode = curvesNode.ChildNodes.Item(j);
                    string curveName = curveNode.Attributes["name"].Value;
                    int numKeys = int.Parse(curveNode.Attributes["num_keys"].Value);
                    string curveNodeBodyString = curveNode.InnerText;
                    string[] curveKeys = curveNodeBodyString.Split();

                    if (curveKeys.Length >= numKeys * 4)
                    {
                        var bonePoseAnimCurve = new AnimationCurve();
                        float keytime = 0;
                        float keyvalue = 0;
                        float keyslopeIn = 0;
                        float keyslopeOut = 0;
                        for (int k = 0; k < numKeys; ++k)
                        {
                            var keyI = k * 4;
                            keytime = float.Parse(curveKeys[keyI + 0]);
                            keyvalue = float.Parse(curveKeys[keyI + 1]);
                            keyslopeIn = float.Parse(curveKeys[keyI + 2]);
                            keyslopeOut = float.Parse(curveKeys[keyI + 3]);

                            // Shift the entire animation by the firstKeyTime, which is negative or 0.
                            // Then all key times are >= 0
                            keytime -= firstKeyTime;

                            bonePoseAnimCurve.AddKey(new Keyframe(keytime, keyvalue, keyslopeIn, keyslopeOut));
                        }
                        if (keytime > lastKeyTime)
                        {
                            lastKeyTime = keytime;
                        }
                        Transform bonePoseObject = transform.Find("facefx_controller/facefx " + curveName);
                        if (bonePoseObject != null)
                        {
                            string controller_relative_path = GetRelativePath(bonePoseObject);
                            curveArray.Add(bonePoseAnimCurve);
                            relativePathArray.Add(controller_relative_path);
                        }
                        else
                            Debug.LogError("bonePoseObject is NULL");
                    }
                    else
                    {
                        Debug.Log("There is an error in the XML file.  There are insufficient keys.");
                    }
                }

                for (int j = 0; j < curveArray.Count; ++j)
                {

                    AnimationCurve bonePoseCurve = curveArray[j];
                    // Unity doesn't like evaluating curves before or after the first/last key, so make sure each curve has
                    // keys at the boundaries of the animations. 	
                    int keyCount = bonePoseCurve.keys.Length;
                    if (keyCount > 0)
                    {
                        if (bonePoseCurve.keys[0].time > 0)
                        {
                            bonePoseCurve.AddKey(new Keyframe(0, bonePoseCurve.Evaluate(0)));
                        }
                        if (bonePoseCurve.keys[keyCount - 1].time < lastKeyTime)
                        {
                            bonePoseCurve.AddKey(new Keyframe(lastKeyTime, bonePoseCurve.Evaluate(lastKeyTime)));
                        }
                    }
                    controllerAnimHelper.animclip.SetCurve(relativePathArray[j], typeof(Transform), "localPosition.x", bonePoseCurve);
                }

                // Using Unity Animation events was not a reliable way to trigger audio.													
                //var audioEvent : AnimationEvent = AnimationEvent();
                //audioEvent.functionName  =  "PlayAudio";
                //audioEvent.time = -firstKeyTime;
                //AnimationUtility.SetAnimationEvents(controllerAnimHelper.animclip, [audioEvent] );


                // Store the audio start time in the localPosition.x value of the facefx controller.
                string objectRelativePath = GetRelativePath(facefx_controller);
                AnimationCurve audioStartTimeCurve = new AnimationCurve();
                audioStartTimeCurve.AddKey(new Keyframe(0, -firstKeyTime));
                audioStartTimeCurve.AddKey(new Keyframe(lastKeyTime, -firstKeyTime));
                controllerAnimHelper.animclip.SetCurve(objectRelativePath, typeof(Transform), "localPosition.x", audioStartTimeCurve);

                GetComponent<Animation>().AddClip(controllerAnimHelper.animclip, animGroupName + "_" + animName);
            }
        }
        else
            Debug.LogError("facefx_controller is NULL");
    }

    Transform myParent;

    public void ImportXML(string xmltext)
    {
        //myParent = transform.parent;
        
        var doc_reader = new XmlDocument();
        doc_reader.Load(new StringReader(xmltext));

        // Test to see if this is a FaceFX XML file
        XmlNodeList faceGraphNodes = doc_reader.SelectNodes("/actor/face_graph");
        if (faceGraphNodes.Count > 0)
        {
            // Use the scale factor from the XML file if it exists.
            if (faceGraphNodes.Item(0).ParentNode.Attributes["scalefactor"] != null)
            {
                ScaleFactor = float.Parse(faceGraphNodes.Item(0).ParentNode.Attributes["scalefactor"].Value);
            }

            Transform existing_controller = transform.Find("facefx_controller");
            if (existing_controller != null)
            {
                //Debug.Log("Warning: There was an existing facefx controller.  Deleting.");
                //Destroy(existing_controller.gameObject);
            }
            else
            {
                goFacefx = new GameObject("facefx_controller");
                goFacefx.transform.parent = transform;

                XmlNodeList refBoneList = doc_reader.SelectNodes("/actor/face_graph/bones/bone");

                Hashtable myRefBoneIndexTable = new Hashtable();


                List<BoneTransform> myRefBoneFileTransforms = new List<BoneTransform>();
                List<string> myRefBoneNames = new List<string>();
                List<Transform> myRefBoneGameObjectTransforms = new List<Transform>();
                List<BoneTransform> myRefBoneGameObjectBoneTransforms = new List<BoneTransform>();

                List<Vector3> myRefBoneFilePositions = new List<Vector3>();
                List<Quaternion> myRefBoneFileRotations = new List<Quaternion>();
                List<Vector3> myRefBoneFileScales = new List<Vector3>();

                for (var i = 0; i < refBoneList.Count; ++i)
                {
                    var refBone = refBoneList.Item(i);
                    string refBoneName = refBone.Attributes["name"].Value;
                    myRefBoneNames.Add(refBoneName);

                    string refBoneBodyString = refBone.InnerText;

                    myRefBoneIndexTable[refBoneName] = i;

                    Transform refBoneObjectTransform = RecursiveFind(transform, refBoneName);
                    myRefBoneGameObjectTransforms.Add(refBoneObjectTransform);
                    /*if (refBoneObjectTransform == null)
                    {
                        if (refBoneName.Contains(":"))
                        {
                            refBoneObjectTransform = RecursiveFind(transform, refBoneName.Split(':')[1]);
                        }
                    }
                     */

                    if (refBoneObjectTransform == null)
                    {
                        Debug.Log("Warning: Couldn't find refbone: " + refBoneName + " in " + transform.name, transform);
                        refBoneObjectTransform = transform;
                    }


                    var trans = new BoneTransform(refBone.InnerText);
                    Vector3 myRefBonePos = trans.GetPos();
                    Quaternion myRefBoneQuat = trans.GetRot();
                    Vector3 myRefBoneScale = trans.GetScale();


                    myRefBoneGameObjectBoneTransforms.Add(new BoneTransform(refBoneObjectTransform.localPosition, refBoneObjectTransform.localRotation, refBoneObjectTransform.localScale));

                    myRefBoneFileTransforms.Add(new BoneTransform(Vector3.Scale(myRefBonePos, new Vector3(ScaleFactor, ScaleFactor, ScaleFactor)), myRefBoneQuat, myRefBoneScale));
                    myRefBoneFilePositions.Add(myRefBonePos);
                    myRefBoneFileRotations.Add(myRefBoneQuat);
                    myRefBoneFileScales.Add(myRefBoneScale);

                }

                //transform.parent = null;

                List<string> myBonePoses = new List<string>();
                XmlNodeList nodeList = doc_reader.SelectNodes("/actor/face_graph/nodes/node/bones");
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    var bonesNode = nodeList.Item(i);
                    string bonePoseName = bonesNode.ParentNode.Attributes["name"].Value;
                    myBonePoses.Add(bonePoseName);

                    var facefx_controller_subobject = new GameObject("facefx " + bonePoseName);

                    facefx_controller_subobject.transform.parent = goFacefx.transform;

                    var bonePoseHelper = new AnimClipHelper();

                    for (var j = 0; j < bonesNode.ChildNodes.Count; ++j)
                    {
                        bonePoseHelper.PreAddKeys();

                        var boneNode = bonesNode.ChildNodes.Item(j);
                        var boneName = boneNode.Attributes["name"].Value;
                        int refboneIndex = (int)myRefBoneIndexTable[boneName];
                        if (myRefBoneIndexTable == null)
                        {
                            Debug.Log("Warning! Bone not in reference pose! " + boneName);
                        }
                        else
                        {
                            Transform boneObject = myRefBoneGameObjectTransforms[refboneIndex];
                            if (boneObject)
                            {
                                string bodyString = boneNode.InnerText;
                                BoneTransform boneTrans = new BoneTransform(bodyString);

                                // Scale bone poses by ScaleFactor
                                Vector3 boneTransPos = Vector3.Scale(new Vector3(boneTrans.Values[0], boneTrans.Values[1], boneTrans.Values[2]), new Vector3(ScaleFactor, ScaleFactor, ScaleFactor));
                                boneTrans.Values[0] = boneTransPos.x;
                                boneTrans.Values[1] = boneTransPos.y;
                                boneTrans.Values[2] = boneTransPos.z;

                                if (UseReferencePoseFromFBX)
                                {
                                    // Calculate the difference between the reference pose in the xml file and the bone pose, then apply the difference to what's in the FBX
                                    Vector3 pos = boneTrans.GetPos() - myRefBoneFilePositions[refboneIndex] + boneObject.transform.localPosition;

                                    Quaternion quat = Quaternion.Inverse(myRefBoneFileRotations[refboneIndex]) * boneTrans.GetRot();
                                    quat = boneObject.transform.localRotation * quat;

                                    // Probably overkill...I'm not sure if non-uniform scale is even supported in Unity.
                                    Vector3 fp = boneTrans.GetScale();
                                    Vector3 fr = myRefBoneFileScales[refboneIndex];
                                    Vector3 gr = boneObject.transform.localScale;
                                    Vector3 scale = new Vector3(fp.x * gr.x / fr.x, fp.y * gr.y / fr.y, fp.z * gr.z / fr.z);

                                    //var scale:Vector3 = boneObject.transform.localScale;
                                    bonePoseHelper.AddKeys(0, myRefBoneGameObjectBoneTransforms[refboneIndex]);
                                    bonePoseHelper.AddKeys(1, new BoneTransform(pos, quat, scale));
                                }
                                else
                                {
                                    bonePoseHelper.AddKeys(0, myRefBoneFileTransforms[refboneIndex]);
                                    bonePoseHelper.AddKeys(1, boneTrans);
                                }



                                if (boneObject != null)
                                {
                                    string objectRelativePath = GetRelativePath(boneObject);
                                    bonePoseHelper.PostAddKeys(objectRelativePath);
                                }

                                GetComponent<Animation>().AddClip(bonePoseHelper.animclip, "facefx " + bonePoseName);
                            }
                        }
                    }
                }

                ImportXMLAnimations(xmltext);

                //transform.parent = myParent;

                // Create an animation with only the reference pose to play in the background.
                var loopAnim = new AnimClipHelper();
                for (int i = 0; i < refBoneList.Count; ++i)
                {
                    if (myRefBoneGameObjectTransforms[i] != null)
                    {
                        loopAnim.PreAddKeys();
                        if (UseReferencePoseFromFBX)
                        {
                            loopAnim.AddKeys(0, myRefBoneGameObjectBoneTransforms[i]);
                            loopAnim.AddKeys(1, myRefBoneGameObjectBoneTransforms[i]);
                        }
                        else
                        {
                            loopAnim.AddKeys(0, myRefBoneFileTransforms[i]);
                            loopAnim.AddKeys(1, myRefBoneFileTransforms[i]);
                        }
                        string objectRelativePath = GetRelativePath(myRefBoneGameObjectTransforms[i]);
                        loopAnim.PostAddKeys(objectRelativePath);
                    }
                }
                GetComponent<Animation>().AddClip(loopAnim.animclip, "facefx_loop_anim");

                Debug.Log("Completed importing FaceFX XML file.");
                InitializeFaceFXController(goFacefx);
            }
        }
        else
        {
            Debug.LogError("Failed to post process XML file!");
        }
    }

    static string GetRelativePath(Transform obj)
    {
        if (obj != null)
        {
            string objectRelativePath = obj.name;
            Transform curObject = obj.transform;
            while (curObject.parent != null && curObject.parent.GetComponent<Animation>() == null)
            {
                if (curObject.parent.parent != null)// && curObject.parent.parent.GetComponent<Animation>() == null)
                {
                    objectRelativePath = curObject.parent.name + "/" + objectRelativePath;
                }
                curObject = curObject.parent;
            }
            return objectRelativePath;
        }
        else
        {
            Debug.Log("You asked me to get a relative path for a NULL object!");
        }
        return "";
    }

    void Start()
    {
        //Debug.Log("Starting FaceFX Controller.");
        switch_anim = true;
        play_state = 0;
        goFacefx = transform.Find("facefx_controller").gameObject;
        if (goFacefx == null)
        {
            Debug.Log("Warning.  Could not find FaceFX Controller for " + name + "!  You need to use the ImportXML function and pass a FaceFX XML file.");
        }
        else
        {
            InitializeFaceFXController(goFacefx);
        }
        if (inverse_hermite == null)
        {
            inverse_hermite = new AnimationCurve();
            var hermiteCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

            // The "step" here defines how accurate the inverse_hermite curve is.
            for (float i = 0; i <= 1; i = i + .01f)
            {
                inverse_hermite.AddKey(hermiteCurve.Evaluate(i), i);
            }
        }

        IsReady = true;
    }

    // Stops animation and audio from playing.  Resets the states so animations can be 
    // played again from the start.
    public void StopAnim()
    {
		AnimationState animState = GetComponent<Animation>()[animation_name];
		animState.time = animState.length - 0.1f;
		GetComponent<AudioSource>().time = audio_clip.length - 0.1f;
		/*
        // If we have stopped this animation prematurely, we need to stop the aduio.
        audio.Stop();
        // Setting this to 0 means we are ready to play another animation.
        play_state = 0;
        switch_anim = true;*/
    }
	
    // Facial animations  frequently start before the corresponding audio becuase the mouth needs to 
    // move into the correct position to form the first sound.  The audio start time is stored in the 
    // localPosition.x value of the facefx controller for the particular animation.
    void PlayAudioFunction()
    {
        if (gameObject.activeSelf)
        {
            if (!GetComponent<AudioSource>())
            {
                gameObject.AddComponent<AudioSource>();
            }

            GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
            GetComponent<AudioSource>().minDistance = 0;
            GetComponent<AudioSource>().maxDistance = 50;

            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(audio_clip);

            if (GetComponent<Animation>()[animation_name] != null)
            {
                audio_start_time = GetComponent<Animation>()[animation_name].time;
            }
        }
        else
            Debug.Log("I can't play audio. Gameobject is inactive");
    }

    public void PlayAnim(string animName, AudioClip animAudio)
    {
        StartCoroutine(PlayAnimCoroutine(animName, animAudio));
    }

    // An animation name and an audio clip are passed to the PlayAnim function
    // to start playing a facial animation.
    public IEnumerator PlayAnimCoroutine(string animName, AudioClip animAudio)
    {
        switch_anim = false;
        play_state = 1;
        anim_eval_time = 0;
        audio_start_time = 0;
        if (null == animAudio)
        {
            Debug.Log("Audio is null");
        }
        animation_name = animName;
        audio_clip = animAudio;

        if (animName != null)
        {
            AnimationState animState = GetComponent<Animation>()[animation_name];
            if (animState != null)
            {
                Debug.Log("playing anim " + animName);
                animState.speed = 0;
                animState.time = 0;
                animState.layer = 2;
                GetComponent<Animation>().Play(animName);
                if (goFacefx != null)
                {
                    // Set to a high value so that we don't trigger audio until audio_start_time is correctly set and we are past it.
                    audio_start_time = 1000;
                    // Yield for one frame so that the facefx controller x position (representing the start time of the audio) has time to update with the animation. 
                    yield return new WaitForEndOfFrame();

                    if (audio_clip.name == "madison-fullwin1")
                    {
                        Debug.LogError(goFacefx.transform.localPosition.x, goFacefx.transform);
                    }

                    audio_start_time = goFacefx.transform.localPosition.x;
                }
                else
                    Debug.LogError("facefx_controller is NULL");
            }
            else
                Debug.LogError("No AnimationState for animation:" + animation_name + " on player " + name);
        }

        //PlayAudioFunction();
    }

    // Searches the object this script is attached to recursively to find a match.  We can't use GameObject.Find because that searches the whole scene.  Transform.Find searches one level.
    Transform RecursiveFind(Transform trans, string searchName)
    {
        foreach (Transform child in trans)
        {
            if (child.name == searchName)
            {
                return child;
            }
            Transform returnTransform = RecursiveFind(child, searchName);
            if (returnTransform != null)
            {
                return returnTransform;
            }
        }
        return null;
    }

    void Update()
    {
        if (play_state > 0)
        {
            AnimationState animState = GetComponent<Animation>()[animation_name];
            if (animState != null)
            {
                // We calcualte the animation evaluation time here.  It is overridden by the audio-based time if audio is playing.
                anim_eval_time = anim_eval_time + Time.deltaTime;

                if (play_state == 1)
                {
                    if (audio_clip.name == "madison-fullwin1")
                    {
                        Debug.LogError(animState.time + " " + audio_start_time);
                    }

                    if (animState.time >= audio_start_time)
                    {
                        PlayAudioFunction();
                        play_state = 2;
                    }
                }
                if (play_state == 2)
                {
                    // audio.isPlaying is not a reliable test alone because audio stops when you loose focus.
                    // But without it, the audio.time can reset to 0 when audio is finished.
                    if (GetComponent<AudioSource>().isPlaying && GetComponent<AudioSource>().time < audio_clip.length)
                    {
                        // While audio is playing, assume control of animation playback and force synch it to the audio.
                        anim_eval_time = GetComponent<AudioSource>().time + audio_start_time;
                    }
                    else if (!GetComponent<AudioSource>().isPlaying)
                    {
                        play_state = 3;
                    }
                }

                if (play_state == 3)
                {
                    if (anim_eval_time >= animState.length)
                    {
                        switch_anim = true;
                        play_state = 0;
                    }
                }

                // Only "tick" the animation if it wouldn't put us over the animation bounds.
                if (anim_eval_time <= animState.length)
                {
                    animState.time = anim_eval_time;
                }

                if (goFacefx != null)
                {
                    foreach (Transform child in goFacefx.transform)
                    {
                        AnimationState anim = GetComponent<Animation>()[child.name];
                        if (anim != null)
                        {

                            // The x axix stores this bone pose's curve.  
                            //The normalized time of the animation is from 0-1.  
                            //At 1, the bone pose is fully driven.  At 0, it is the reference pose.
                            // Unfortunately, the interpolation from 0-1 is a hermite curve, not linear. So we use the inverse_hermite
                            // curve to figure out what value we need to pass into the hermite curve evaluation to drive the bone pose by
                            // child.transform.localPosition.x
                            anim.normalizedTime = inverse_hermite.Evaluate(child.transform.localPosition.x);

                            // Remove shaking by setting normalized speed to 0.
                            anim.normalizedSpeed = 0;
                        }
                    }
                }
                else
                    Debug.LogError("facefx_controller is NULL");
            }
            // To support audio playback without animation, reset the state if the animation is null and the audio is finished playing.
            else if (!GetComponent<AudioSource>().isPlaying)
            {
                Debug.Log("audio with no animation case");
                switch_anim = true;
                play_state = 0;
            }
        }
    }

    int GetPlayState()
    {
        return play_state;
    }

    static bool GetSwitchAnim()
    {
        return switch_anim;
    }
}
