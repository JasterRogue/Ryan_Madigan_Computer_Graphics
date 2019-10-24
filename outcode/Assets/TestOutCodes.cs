using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutCodes : MonoBehaviour
{
    static int resWidth = 800;
    static int resHeight = 600;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        Vector3[] imageAfterViewingMatrix = applyViewingMatrix(cube);

        Vector3[] cubeAfterProjectionMatrix = applyProjectionMatrix(imageAfterViewingMatrix);

        cubeAfterProjectionMatrix = divide_by_z(cubeAfterProjectionMatrix);

        Renderer ourScreen = FindObjectOfType<Renderer>();

        Vector2 v1 = new Vector2(-1f, 1f);
        Vector2 v2 = new Vector2(1f, -1f);


        Texture2D screen = new Texture2D(resWidth, resHeight);
        ourScreen.material.mainTexture = screen;

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


        screen.Apply();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
