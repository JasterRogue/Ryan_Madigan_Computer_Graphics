using UnityEngine;
using System.Collections;
using System;

public class Transformations : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);


		//Rotation Matrix
        Vector3 startingAxis = new Vector3(15, 2, 2);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(49, startingAxis);
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(new Vector3(0,0,0),
                            rotation,
                            Vector3.one);
        printMatrix(rotationMatrix);

        Vector3[] imageAfterRotation =
            MatrixTransform(cube, rotationMatrix);
        printVerts(imageAfterRotation);
        
		//Scale Matrix
		//Vector3 startingAxis = new Vector3(15, 2, 2);
		startingAxis.Normalize();
		//Quaternion rotation = Quaternion.AngleAxis(49, startingAxis);
		Matrix4x4 scaleMatrix =
			Matrix4x4.TRS(new Vector3(0,0,0),
				Quaternion.identity,
				new Vector3(15,4,2));
		printMatrix(scaleMatrix);

		Vector3[] imageAfterScale =
			MatrixTransform(imageAfterRotation, scaleMatrix);
		printVerts(imageAfterScale);

        //Transform Matrix
        //Vector3 startingAxis = new Vector3(15, 2, 2);
        startingAxis.Normalize();
        //Quaternion rotation = Quaternion.AngleAxis(49, startingAxis);
        Matrix4x4 transformMatrix =
            Matrix4x4.TRS(new Vector3(4, 4, 3),
                Quaternion.identity,
                 Vector3.one);
        printMatrix(transformMatrix);

        Vector3[] imageAfterTransform =
            MatrixTransform(imageAfterScale, transformMatrix);
        printVerts(imageAfterTransform);

        //Single Matrix of transforms
        Matrix4x4 singleMatrixOfTransformations = transformMatrix  * scaleMatrix * rotationMatrix;

        Vector3[] imageAfterMatrixOfTransforms = MatrixTransform(cube, singleMatrixOfTransformations);

        //Viewing Matrix
        Matrix4x4 viewingMatrix = Matrix4x4.TRS(new Vector3(-17,-5,-52), Quaternion.LookRotation(new Vector3(2,15,2) - new Vector3(17,5,51).normalized),Vector3.one);

        Vector3[] imageAfterViewingMatrix = MatrixTransform(imageAfterTransform, viewingMatrix);

        //Projection Matrix
        Matrix4x4 projectionMatrix = Matrix4x4.Perspective(45, 1.6f, 1, 1000);

        Vector3[] imageAfterProjection = MatrixTransform(imageAfterViewingMatrix, projectionMatrix);

        //Final Matrix
        Matrix4x4 finalMatrix = projectionMatrix * viewingMatrix * transformMatrix * scaleMatrix * rotationMatrix;

        Vector3[] imageAfterFinalMatrix = MatrixTransform(cube, finalMatrix);

    }

    private void printVerts(Vector3[] newImage)
    {
        for (int i = 0; i < newImage.Length; i++)
            print(newImage[i].x + " , " +
                newImage[i].y + " , " +
                newImage[i].z);

    }

    private Vector3[] MatrixTransform(
        Vector3[] meshVertices, 
        Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix * 
                new Vector4( 
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;
    }

    private void printMatrix(Matrix4x4 matrix)
    {
        for (int i = 0; i < 4; i++)
            print(matrix.GetRow(i).ToString());
    }



    // Update is called once per frame
    void Update () {
	
	}
}
