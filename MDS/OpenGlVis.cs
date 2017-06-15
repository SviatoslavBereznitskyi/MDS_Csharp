using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using OpenGL;

namespace MDS
{
   
    class OpenGlVis
    {
        private static int width = 1200, height = 720;
        private static ShaderProgram program;
        private static VBO<Vector3> cubeNormals;
        private static List<VBO<Vector3>> cubes;
        private static VBO<Vector2> cubeUV;
        private static VBO<int> cubeQuads;
        private static List<Texture> crateTexture;
        private static System.Diagnostics.Stopwatch watch;
        private static float xangle, yangle, zoomV=100;
        private static bool autoRotate, lighting = true, fullscreen = false;
        private static bool left, right, up, down,zoomP,zoomM;
        private static List<PointArt3d> input1;
        private static bool isFirst=true;
        //private static string FSMode = "1366x768:32@60";
        private static void InitGlut() 
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("3d");
            
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
           // Gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
           // Glut.glutc(0.8, 1.0, 0.6, 1.0);
            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);

            Glut.glutMouseFunc(OnMouse);
            Glut.glutMotionFunc(OnMove);

            Glut.glutCloseFunc(OnClose);
            Glut.glutReshapeFunc(OnReshape);
           
            Gl.Enable(EnableCap.DepthTest);

            program = new ShaderProgram(VertexShader, FragmentShader);

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 180), Vector3.Zero, Vector3.Up));
            crateTexture = new List<Texture>();
            program["light_direction"].SetValue(new Vector3(0, -0.5, 1));
            program["enable_lighting"].SetValue(lighting);
            isFirst = false;
        
        }
       public static void Start(List<PointArt3d> input)
        {
            if (isFirst)
            {
                InitGlut();
            }
           input1 = input;
           
            for (int i = 0; i < 14; i++)
            {
                crateTexture.Add(new Texture(String.Format("{0}.jpg",i)));
            }
            //crateTexture = new Texture("crate.jpg");
            cubes = new List<VBO<Vector3>>();
            
            for (int i = 0; i< input.Count; i++)
            {
                double x = input[i].X;
                double y = input[i].Y;
                double z = input[i].Z;

                cubes.Add(new VBO<Vector3>(new Vector3[] {
                new Vector3(1+x, 1+y, -1+z), new Vector3(-1+x, 1+y, -1+z), new Vector3(-1+x, 1+y, 1+z), new Vector3(1+x, 1+y, 1+z),         // top
                new Vector3(1+x, -1+y, 1+z), new Vector3(-1+x, -1+y, 1+z), new Vector3(-1+x, -1+y, -1+z), new Vector3(1+x, -1+y, -1+z),     // bottom
                new Vector3(1+x, 1+y, 1+z), new Vector3(-1+x, 1+y, 1+z), new Vector3(-1+x, -1+y, 1+z), new Vector3(1+x, -1+y, 1+z),         // front face
                new Vector3(1+x, -1+y, -1+z), new Vector3(-1+x, -1+y, -1+z), new Vector3(-1+x, 1+y, -1+z), new Vector3(1+x, 1+y, -1+z),     // back face
                new Vector3(-1+x, 1+y, 1+z), new Vector3(-1+x, 1+y, -1+z), new Vector3(-1+x, -1+y, -1+z), new Vector3(-1+x, -1+y, 1+z),     // left
                new Vector3(1+x, 1+y, -1+z), new Vector3(1+x, 1+y, 1+z), new Vector3(1+x, -1+y, 1+z), new Vector3(1+x, -1+y, -1+z) }));      // right
            }
            cubeNormals = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), 
                new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), 
                new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), 
                new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), 
                new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), 
                new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0) });
            cubeUV = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });


            cubeQuads = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);


            watch = System.Diagnostics.Stopwatch.StartNew();
            Glut.glutShowWindow();
            Glut.glutMainLoop();
           
          
        }
       private static bool mouseDown = false;
       private static int downX, downY;
       private static int prevX, prevY;
       private static void OnMouse(int button, int state, int x, int y)
       {
           if (button != Glut.GLUT_LEFT_BUTTON) return;

           // Цей метод викликається щоразу, коли натискається кнопка миші
           mouseDown = (state == Glut.GLUT_DOWN);

           // при натиску зберігаємо значення положення курсора
           if (mouseDown)
           {
               Glut.glutSetCursor(Glut.GLUT_CURSOR_NONE);
               prevX = downX = x;
              prevY = downY = y;
           }
           else // відобразити курсор при відпуканні кнопки
           {
              
               Glut.glutSetCursor(Glut.GLUT_CURSOR_LEFT_ARROW);
               Glut.glutWarpPointer(downX, downY);
           }
       }

       private static void OnMove(int x, int y)
       {
           // при виклику миші з glutWarpPointer нічого не робити
           if (x == prevX && y == prevY) return;

           // повертати камеру при натисканні кнопки миші
           if (mouseDown)
           {
               //float yaw = (prevX - x) * 0.002f;
               ////camera.Yaw(yaw);
               //float pitch = (prevY - y) * 0.002f;
               xangle += ((prevY - y) * 0.002f);
               yangle += ((prevX - x) * 0.002f);
               
               ///camera.Pitch(pitch);
               prevX = x;
               prevY = y;
           }

           if (x < 0) { Glut.glutWarpPointer(prevX =width-20, y); }
           else if (x > width - 20) { Glut.glutWarpPointer(prevX = 0, y); }

           if (y < 0) { Glut.glutWarpPointer(x, prevY =height-20); }
           else if (y > height-20) { Glut.glutWarpPointer(x, prevY = 0); }
       }

        private static void OnClose()
        {
            foreach (var item in cubes)
            {
                item.Dispose();
            }

            cubeNormals.Dispose();
            cubeUV.Dispose();
            cubeQuads.Dispose();
            foreach (var item in crateTexture)
            {
                item.Dispose();
            }
            
           
        }

        private static void OnDisplay()
        {

        }

        private static void OnRenderFrame()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();
           //автоповорот
            if (autoRotate)
            {
                yangle += deltaTime/2;
            }
            if (right) yangle += deltaTime;
            if (left) yangle -= deltaTime;
            if (up) xangle -= deltaTime;
            if (down) xangle += deltaTime;
            if (zoomP) zoomV+=0.5f;
            if (zoomM) zoomV -= 0.5f;
            // встановлення параметрів точки огляду
            Gl.Viewport(0, 0, width, height);
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, zoomV), Vector3.Zero, Vector3.Up));
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //інціалізаціяя шейдерів
            Gl.UseProgram(program);

            program["model_matrix"].SetValue(Matrix4.CreateRotationY(yangle ) * Matrix4.CreateRotationX(xangle ));//шейдер обертання
            program["enable_lighting"].SetValue(lighting);                                                      //шейдер світла
            //ініціалізація моделі
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeQuads);
            for (int i = 0; i < cubes.Count; i++)
			{
                Gl.BindTexture(crateTexture[input1[i].IdCat+1]); 
                Gl.BindBufferToShaderAttribute(cubes[i], program, "vertexPosition");
                //Gl.BindBufferToShaderAttribute(cubes[i], program, "vertexPosition");
                Gl.DrawElements(BeginMode.Quads, 48, DrawElementsType.UnsignedInt, IntPtr.Zero);//малювання поточного елмента
            }
            Glut.glutSwapBuffers();
        }

        private static void OnReshape(int width, int height)
        {
         

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
        }

        private static void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 'w') up = true;
            else if (key == 's') down = true;
            else if (key == 'd') right = true;
            else if (key == 'a') left = true;
            else if (key == 'z') zoomP = true;
            else if (key == 'x') zoomM = true;
            else if (key == 27) Glut.glutHideWindow();
        }

        private static void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == 'w') up = false;
            else if (key == 's') down = false;
            else if (key == 'd') right = false;
            else if (key == 'a') left = false;
            else if (key == 'z') zoomP = false;
            else if (key == 'x') zoomM = false;
            else if (key == ' ') autoRotate = !autoRotate;
            else if (key == 'l') lighting = !lighting;
            else if (key == 'f')
            {
    string s = String.Format("{0}x{1}:{2}@{3}",Glut.GLUT_GAME_MODE_WIDTH,Glut.GLUT_GAME_MODE_HEIGHT,Glut.GLUT_GAME_MODE_PIXEL_DEPTH,Glut.GLUT_GAME_MODE_REFRESH_RATE);
                fullscreen = !fullscreen;
                if (fullscreen)
                {
                   
                    Glut.glutFullScreen();
                }
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                    //Glut.glutLeaveGameMode();
                    //  Glut.glutFullScreen();
                }
            }
        }

        private static void glutGameModeString(string FSMode)
        {
            throw new NotImplementedException();
        }

        public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    normal = normalize((model_matrix * vec4(floor(vertexNormal), 0)).xyz);
    uv = vertexUV;

    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"
#version 130

uniform sampler2D texture;
uniform vec3 light_direction;
uniform bool enable_lighting;
uniform vec3 color;

in vec3 normal;
in vec2 uv;

out vec4 fragment;

void main(void)
{
    float diffuse = max(dot(normal, light_direction), 0);
    float ambient = 0.3;
    float lighting = (enable_lighting ? max(diffuse, ambient) : 1);
//fragment = vec4(color * texture2D(texture, uv).xyz, 1);
    fragment = lighting * texture2D(texture, uv);
}
";
    }
}
