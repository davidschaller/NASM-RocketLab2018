using UnityEngine;
using System.Collections;

public class TestComputeShader : MonoBehaviour 
{
   public ComputeShader computeShader; // drag&drop ton fichier

   void Start()
   {
      // init tableaux
      float[] inputData = new float[] {1, 2, 3, 4, 5};
      float[] outputData = new float[5];

      // init buffer
      ComputeBuffer inputBuffer = new ComputeBuffer(5, sizeof(float));
      ComputeBuffer outputBuffer = new ComputeBuffer(5, sizeof(float));

      // bind tableau au buffer
      inputBuffer.SetData(inputData );

      //bind buffer au compute shader
      computeShader.SetBuffer (0, "myInput", inputBuffer );
      computeShader.SetBuffer (0, "myOutput", outputBuffer );

      //lancement du kernel
      computeShader.Dispatch(0, 5, 1, 1);
   
      // lecture de la sortie (pas de fonction wait (?))
      outputBuffer.GetData(outputData );

      // affichage
      foreach (float f in outputData)
         Debug.Log(f);
		
	  inputBuffer.Release();
	  outputBuffer.Release();
   }
}
