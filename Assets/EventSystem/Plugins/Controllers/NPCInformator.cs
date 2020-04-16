
using UnityEngine;
public class NPCInformator : NPCController
{
    public string location = "Bank";
    public EventPlayerBase interviewEventPlayer;

    public string interviewButtonText = "Interview to ";

    private Quaternion oldRotation;

    private void OnGUI()
    {
        if (IsLocked)
            return;

        if (MovementController.ControlTarget == null)
        {
            Debug.Log("ControlTarget is NULL");
            return;
        }

        Vector2 screenPos = 
            GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2));

        float dist = (transform.position - MovementController.ControlTarget.GetTransform().position).magnitude;

        if (dist < maxTalkDistance && interviewEventPlayer != null)
        {
            string text = interviewButtonText + npcName;

            Vector2 buttonSize = GUI.skin.GetStyle(GUIManager.NPCNameButtonStyle).CalcSize(new GUIContent(text));
            Rect buttonRect = new Rect(screenPos.x - buttonSize.x / 2, 
                    Screen.height - screenPos.y - buttonSize.y, buttonSize.x, buttonSize.y);

            if (GUI.Button(buttonRect, text, GUIManager.NPCNameButtonStyle))
            {
                Debug.Log("Interview button is clicked");

                if (interviewEventPlayer != null)
                {
                    SV_NPCInformator forcedNPC = interviewEventPlayer.transform.GetComponent<SV_NPCInformator>();
                    forcedNPC.substitutionInformatorNPC = this;

                    interviewEventPlayer.PlayerTriggered();
                }
                else
                    Debug.Log("interviewEventPlayer is NULL. Can't start interview");
            }
        }
    }

    public void LookAt(Vector3 p_pos)
    {
        Debug.Log("Look at " + p_pos);

        oldRotation = transform.rotation;

        transform.LookAt(p_pos);
    }

    public void RestoreRotation()
    {
        transform.rotation = oldRotation;
    }

    public void PlayAnswerAnimation()
    {
        Debug.Log("Answer the question");
        Actor.PlayAnimationOnce("walkgoofy");
    }
}

