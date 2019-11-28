using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutCodes : MonoBehaviour
{
    static int resWidth = Screen.width;
    static int resHeight = Screen.height;
    Vector3[] cube;
    Vector3[] imageAfterRotation;
    Vector3[] imageAfterTranslate;
    Texture2D screen;
    private float angle;
    Color defaultColour;
    

    Renderer ourScreen;
    // Start is called before the first frame update
    void Start()
    {
        ourScreen = FindObjectOfType<Renderer>();
        screen = new Texture2D(resWidth, resHeight);
        defaultColour = new Color(screen.GetPixel(1, 1).r, screen.GetPixel(1, 1).g, screen.GetPixel(1, 1).b, screen.GetPixel(1, 1).a);
        ourScreen.material.mainTexture = screen;
        

        cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        //Cube needed for using drawCubeV2()
        cube[0] = new Vector3(-1, 1, 1);
        cube[1] = new Vector3(1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(-1, 1, -1);
        cube[5] = new Vector3(1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        Vector3[] imageAfterViewingMatrix = applyViewingMatrix(cube);

        Vector3[] cubeAfterProjectionMatrix = applyProjectionMatrix(imageAfterViewingMatrix);

      //  cubeAfterProjectionMatrix = applyTranslateMAtrix(cubeAfterProjectionMatrix);

        imageAfterRotation = applyRotationMatrix(cubeAfterProjectionMatrix);

        cubeAfterProjectionMatrix = divide_by_z(cubeAfterProjectionMatrix);

        Vector2 v1 = new Vector2(-1f, 1f);
        Vector2 v2 = new Vector2(1f, -1f);    

        drawCubeV2(cubeAfterProjectionMatrix, screen);


       // floodFill(resWidth / 2, resHeight / 2, Color.yellow, defaultColour, screen);

        screen.Apply();
        
    }
    private Matrix4x4 translateMatrix(Vector3 v)
    {
        return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
    }
    private Matrix4x4 rotationMatrix(Vector3 axis, float angle)
    {  
        Quaternion rotation = Quaternion.AngleAxis(angle, axis.normalized);
        return
            Matrix4x4.TRS(new Vector3(0, 0, 0),
                            rotation,
                            Vector3.one);
    }

    private Matrix4x4 viewingMatrix(Vector3 positionOfCamera, Vector3 target, Vector3 up)
    {
        return Matrix4x4.TRS(-positionOfCamera, Quaternion.LookRotation(target - positionOfCamera, up.normalized), Vector3.one);
    }

    private Vector3[] applyRotationMatrix(Vector3[] cube)
    {
        Vector3 startingAxis = new Vector3(15, 2, 2);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(49, startingAxis);
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(new Vector3(0, 0, 0),
                            rotation,
                            Vector3.one);
        //printMatrix(rotationMatrix);

        Vector3[] imageAfterRotation =
            MatrixTransform(cube, rotationMatrix);

        return imageAfterRotation;
    }

    public Vector3 calculateIfClockWiseOrAntiClockWise(Vector3 v, Vector3 u)
    {
        return Vector3.Cross(v, u).normalized;
    }

    public void drawCube(Vector3[] cubeAfterProjectionMatrix, Texture2D screen)
    {
        drawLine(cubeAfterProjectionMatrix[0], cubeAfterProjectionMatrix[1], screen);
        drawLine(cubeAfterProjectionMatrix[1], cubeAfterProjectionMatrix[2], screen);
        drawLine(cubeAfterProjectionMatrix[2], cubeAfterProjectionMatrix[3], screen);
        drawLine(cubeAfterProjectionMatrix[3], cubeAfterProjectionMatrix[0], screen);

        drawLine(cubeAfterProjectionMatrix[4], cubeAfterProjectionMatrix[5], screen);
        drawLine(cubeAfterProjectionMatrix[5], cubeAfterProjectionMatrix[6], screen);
        drawLine(cubeAfterProjectionMatrix[6], cubeAfterProjectionMatrix[7], screen);
        drawLine(cubeAfterProjectionMatrix[7], cubeAfterProjectionMatrix[4], screen);

        drawLine(cubeAfterProjectionMatrix[0], cubeAfterProjectionMatrix[4], screen);
        drawLine(cubeAfterProjectionMatrix[1], cubeAfterProjectionMatrix[5], screen);
        drawLine(cubeAfterProjectionMatrix[2], cubeAfterProjectionMatrix[6], screen);
        drawLine(cubeAfterProjectionMatrix[3], cubeAfterProjectionMatrix[7], screen);

    }

    /*This method is for testing to try and get the floodfill to work completely
     If this doesn't work I will just revert back to first drawCube() */
    public void drawCubeV2(Vector3[] cube, Texture2D screen)
    {
        Vector3 frontCross = Vector3.Cross(cube[0], cube[2]);

        if(frontCross.z > 0)
        {
            //front1
            drawLine(cube[0], cube[3], screen);
            drawLine(cube[3], cube[1], screen);
            drawLine(cube[1], cube[0], screen);

            //Front2
            drawLine(cube[0], cube[2], screen);
            drawLine(cube[2], cube[3], screen);
            drawLine(cube[3], cube[0], screen);
        }


        Vector3 leftCross = Vector3.Cross(cube[4], cube[7]);

        if(leftCross.z > 0)
        {
            //Left1
            drawLine(cube[4], cube[2], screen);
            drawLine(cube[2], cube[0], screen);
            drawLine(cube[0], cube[4], screen);

            //Left2
            drawLine(cube[4], cube[6], screen);
            drawLine(cube[6], cube[2], screen);
            drawLine(cube[2], cube[4], screen);
        }

        Vector3 topCross = Vector3.Cross(cube[4], cube[5]);

        if(topCross.z > 0)
        {
            //Top1
            drawLine(cube[4], cube[0], screen);
            drawLine(cube[0], cube[1], screen);
            drawLine(cube[1], cube[4], screen);

            //Top2
            drawLine(cube[4], cube[1], screen);
            drawLine(cube[1], cube[5], screen);
            drawLine(cube[5], cube[4], screen);
        }

        Vector3 backCross = Vector3.Cross(cube[4], cube[6]);

        if(backCross.z > 0)
        {
            //Back1
            drawLine(cube[4], cube[5], screen);
            drawLine(cube[5], cube[7], screen);
            drawLine(cube[7], cube[4], screen);

            //Back2
            drawLine(cube[4], cube[7], screen);
            drawLine(cube[7], cube[6], screen);
            drawLine(cube[6], cube[4], screen);
        }

        Vector3 rightCross = Vector3.Cross(cube[5], cube[7]);

        if(rightCross.z > 0)
        {
            //Right1
            drawLine(cube[5], cube[1], screen);
            drawLine(cube[1], cube[3], screen);
            drawLine(cube[3], cube[5], screen);

            //Right2
            drawLine(cube[5], cube[3], screen);
            drawLine(cube[3], cube[7], screen);
            drawLine(cube[7], cube[5], screen);
        }

        Vector3 baseCross = Vector3.Cross(cube[6], cube[3]);

        if(baseCross.z > 0)
        {
            //Base1
            drawLine(cube[6], cube[3], screen);
            drawLine(cube[3], cube[2], screen);
            drawLine(cube[2], cube[6], screen);

            //Base2
            drawLine(cube[6], cube[7], screen);
            drawLine(cube[7], cube[3], screen);
            drawLine(cube[3], cube[6], screen);
        }

    }



    private Vector3[] applyProjectionMatrix(Vector3[] imageAfterViewingMatrix)
    {
        Matrix4x4 projectionMatrix = Matrix4x4.Perspective(45, 1.6f, 1, 1000);

        Vector3[] imageAfterProjection = MatrixTransform(imageAfterViewingMatrix, projectionMatrix);

        return imageAfterProjection;
;
    }

    private Vector3[] applyViewingMatrix(Vector3[] cube)
    {
        Matrix4x4 viewingMatrix = Matrix4x4.TRS(new Vector3(0, 0, 10), Quaternion.LookRotation(new Vector3(0, 0, 0) - new Vector3(0, 0, 10), new Vector3(0,1,0).normalized), Vector3.one);

        Vector3[] imageAfterViewingMatrix = MatrixTransform(cube, viewingMatrix);

        return imageAfterViewingMatrix;
    }

    public Vector3[] applyTranslateMAtrix(Vector3[] cube)
    {
        Matrix4x4 transformMatrix = Matrix4x4.TRS(new Vector3(4,3,3),Quaternion.identity,Vector3.one);

        Vector3[] imageAfterTranslate = MatrixTransform(cube, transformMatrix);

        return imageAfterTranslate;
    }

    private Vector3[] divide_by_z(Vector3[] cube)
    {
        List<Vector3> output = new List<Vector3>();
        foreach (Vector3 v in cube)
            output.Add(new Vector3(-v.x / v.z, -v.y / v.z, -1.0f));
        return output.ToArray();
    }

    private void drawLine(Vector2 v1,Vector2 v2, Texture2D screen)
    {
        Vector2 start = v1, end = v2;

        if (lineClip(ref start, ref end))
        {
            plot(screen, breshenham(convertXY(start), convertXY(end)));
        }
       
    }

    private void plot(Texture2D screen, List<Vector2Int> list)
    {
        foreach (Vector2Int point in list)
            screen.SetPixel(point.x, point.y, Color.blue);

    }

    public static Vector2Int convertXY(Vector3 v)
    {
        return new Vector2Int((int) ( (v.x + 1.0f)  * (resWidth - 1)/2.0f) , (int)(  (1.0f - v.y)  * (resHeight - 1) / 2.0f));
    }

    public static bool lineClip(ref Vector2 v, ref Vector2 u)
    {

        Outcode v_outcode = new Outcode(v);
        Outcode u_outcode = new Outcode(u);
        Outcode inViewport = new Outcode();
        if ((v_outcode + u_outcode) == inViewport)
            return true;
        if ((v_outcode * u_outcode) != inViewport)
            return false;

        if (v_outcode == inViewport)
            return lineClip(ref u, ref v);

        // v must be outside viewport

        if (v_outcode.up)
        {
            v = intercept(u, v, 0);
            Outcode v2_outcode = new Outcode(v);
            if (v2_outcode == inViewport) return lineClip(ref u, ref v);
        }


        if (v_outcode.down)
        {
            v = intercept(u, v, 1);
            Outcode v2_outcode = new Outcode(v);
            if (v2_outcode == inViewport) return lineClip(ref u, ref v);
        }

        if (v_outcode.left)
        {
            v = intercept(u, v, 2);
            Outcode v2_outcode = new Outcode(v);
            if (v2_outcode == inViewport) return lineClip(ref u, ref v);
        }


        if (v_outcode.right)
        {
            v = intercept(u, v, 3);
            Outcode v2_outcode = new Outcode(v);
            if (v2_outcode == inViewport) return lineClip(ref u, ref v);
        }

        return false; 
    }

    private static Vector2 intercept(Vector2 u, Vector2 v, int edge)
    {
        float m = (v.y - u.y) / (v.x - u.x);

        if (edge == 0)
            return new Vector2(u.x + (1 / m) * (1 - u.y), 1);
        if (edge == 1)
            return new Vector2(u.x + (1 / m) * (-1 - u.y), -1);
        if (edge == 2)
            return new Vector2(-1,  u.y + m * (-1 - u.x));

        return new Vector2(1, u.y + m * (1 - u.x));
    }

    public static List<Vector2Int> breshenham(Vector2Int start, Vector2Int finish)
    {
        List<Vector2Int> breshenhamList = new List<Vector2Int>();

        int dX = finish.x - start.x;
        int dY = finish.y - start.y;
        int twoDY = dY * 2;
        int twoDydX = 2 * (dY - dX);
        int p = twoDY - dX;

        if(dX < 0)
        {
            return breshenham(finish, start);
        }

        if(dY < 0)
        {
            return negativeY(breshenham(negativeY(start), negativeY(finish)));
        }

        if(dY > dX)
        {
            return swapXY(breshenham(swapXY(start), swapXY(finish)));
        }

        int y = start.y;

        for (int x = start.x; x <= finish.x; x++)
        {
            breshenhamList.Add(new Vector2Int(x, y));

            if(p > 0)
            {
                y++;
                p += twoDydX;
            }

            else
            {
                p += twoDY;
            }


        }

        return breshenhamList;
    }//end of bresenham method

    public static List<Vector2Int> negativeY(List<Vector2Int> yValues)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach(Vector2Int v in yValues)
        {
            outputList.Add(negativeY(v));
        }

        return outputList;
    }

    public static Vector2Int negativeY(Vector2Int point)
    {
        return new Vector2Int(point.x, point.y * -1);
    }

    public static List<Vector2Int> swapXY(List<Vector2Int> list)
    {

        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach (Vector2Int v in list)
            outputList.Add(swapXY(v));


        return outputList;
    }

    public static Vector2Int swapXY(Vector2Int value)
    {
        return new Vector2Int(value.y, value.x);
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

    public void floodFill(int x, int y, Color newColour, Color oldColour, Texture2D screen)
    {

       if((x < 0) || (x >= resWidth))
        {
            return;
        }

      if((y < 0) || (y >= resHeight))
        {
            return;
        }

      if(screen.GetPixel(x,y) != oldColour)
        {
            return;
        }

      else 
        {
            screen.SetPixel(x, y, newColour);
            floodFill(x + 1, y, newColour, oldColour, screen);
            floodFill(x, y + 1, newColour, oldColour, screen);
            floodFill(x - 1, y, newColour, oldColour, screen);
            floodFill(x, y - 1, newColour, oldColour, screen);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(screen);
        screen = new Texture2D(resWidth, resHeight);
        ourScreen.material.mainTexture = screen;
        angle += 1;
        Matrix4x4 persp = Matrix4x4.Perspective(45, 1.6f, 1, 1000);
        Matrix4x4 View = viewingMatrix(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix4x4 world = rotationMatrix(new Vector3(1, 1, 0), angle) * translateMatrix(new Vector3(2, 1, 3));;
        Matrix4x4 overAll =persp * View*world;
       
        drawCubeV2(divide_by_z(MatrixTransform(cube, overAll)), screen);

        //floodFill(resWidth/2, resHeight/2, Color.yellow, defaultColour, screen);

        screen.Apply();
    }
}
