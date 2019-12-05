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
    public Light light;
    Vector3[] originalPoints;
    

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
        /*cube[0] = new Vector3(-1, 1, 1);
        cube[1] = new Vector3(1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(-1, 1, -1);
        cube[5] = new Vector3(1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);*/

        //Vector3[] imageAfterViewingMatrix = applyViewingMatrix(cube);

       // Vector3[] cubeAfterProjectionMatrix = applyProjectionMatrix(imageAfterViewingMatrix);

      //  cubeAfterProjectionMatrix = applyTranslateMAtrix(cubeAfterProjectionMatrix);

        //imageAfterRotation = applyRotationMatrix(cubeAfterProjectionMatrix);

        //cubeAfterProjectionMatrix = divide_by_z(cubeAfterProjectionMatrix);

       // Vector2 v1 = new Vector2(-1f, 1f);
       // Vector2 v2 = new Vector2(1f, -1f);   

       // floodFill(resWidth / 2, resHeight / 2, Color.yellow, defaultColour, screen);

        //screen.Apply();
        
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

    private void drawCube(Vector3[] cube)
    {
        //Front
        //t1
        drawFace(cube[0], cube[1], cube[2]);

        //t2
        drawFace(cube[0], cube[2], cube[3]);

        //Right

        //t1
        drawFace(cube[4], cube[0], cube[3]);
        //t2
        drawFace(cube[4], cube[3], cube[7]);

        //Top

        //t1
        drawFace(cube[4], cube[5], cube[1]);

        //t2
        drawFace(cube[4], cube[1], cube[0]);

        //Back

        //t1
        drawFace(cube[5], cube[4], cube[7]);

        //t2
        drawFace(cube[5], cube[7], cube[6]);

        //Left

        //t1
        drawFace(cube[1], cube[5], cube[6]);

        //t2
        drawFace(cube[1], cube[6], cube[2]);

        //Bottom

        //t1
        drawFace(cube[6], cube[7], cube[3]);

        //t2
        drawFace(cube[6], cube[3], cube[2]);
    }

    public Vector3 getVectorNormal(Vector2 a, Vector2 b, Vector2 c)
    {
        return Vector3.Normalize(Vector3.Cross(b - a, c - a));
    }

    public Vector3 getLightDirection(Vector3 center)
    {
        return Vector3.Normalize((center - light.transform.position));
    }

    public void drawFace(Vector2 i, Vector2 j, Vector2 k)
    {
        float z = (j.x - i.x) * (k.y - j.y) - (j.y - i.y)*(k.x - j.x);

        if(z >= 0)
        {
            drawLine(i, j, screen);
            drawLine(j, k, screen);
            drawLine(k, i, screen);

            Vector2 center = getCenter(i, j, k);
            center = convertXY(center);
            Vector3 normal = getVectorNormal(i,j,k);
            Vector3 lightDirection = getLightDirection(center);
            float dotProduct = Vector3.Dot(lightDirection, normal);
            //Color reflection = new Color(dotProduct * Color.yellow.r * light.intensity, dotProduct * Color.yellow.g * light.intensity, dotProduct * Color.yellow.b * light.intensity, 1);
            floodFillStack((int)center.x, (int)center.y, Color.yellow, defaultColour);
        }
    }

    private void floodFillStack(int x, int y, Color newColour, Color oldColour)
    {
        Stack<Vector2> pixels = new Stack<Vector2>();
        pixels.Push(new Vector2(x, y));

        while(pixels.Count > 0)
        {
            Vector2 p = pixels.Pop();
            if(checkBounds(p))
            {
                if(screen.GetPixel((int)p.x, (int)p.y) == oldColour)
                {
                    screen.SetPixel((int)p.x, (int)p.y, newColour);
                    pixels.Push(new Vector2(p.x + 1, p.y));
                    pixels.Push(new Vector2(p.x - 1, p.y));
                    pixels.Push(new Vector2(p.x, p.y + 1));
                    pixels.Push(new Vector2(p.x, p.y - 1));
                }
            }
        }
    }

    private Vector2 getCenter(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return new Vector2((p1.x + p2.x + p3.x) / 3, (p1.y + p2.y + p3.y) / 3);
    }

    public bool checkBounds(Vector2 pixel)
    {
        if((pixel.x < 0) || (pixel.x >= resWidth - 1))
        {
            return false;
        }

        if((pixel.y < 0) || (pixel.y >= resHeight - 1))
        {
            return false;
        }

        return true;
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
        Matrix4x4 world = rotationMatrix(new Vector3(1, 1, 0), angle) /* * translateMatrix(new Vector3(2, 1, 3))*/;
        Matrix4x4 overAll =persp * View*world;

        originalPoints = cube;
       
        drawCube(divide_by_z(MatrixTransform(cube, overAll)));

        //floodFill(resWidth/2, resHeight/2, Color.yellow, defaultColour, screen);

        screen.Apply();
    }
}
