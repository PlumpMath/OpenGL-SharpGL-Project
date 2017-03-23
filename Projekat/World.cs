using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using System.Drawing.Imaging;
using System.Drawing;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph;
using System.Windows.Threading;
using AssimpSample;

namespace Projekat
{
    class World : IDisposable
    {
        private AssimpScene scene;

        private float xRotation = 20.0f;

        private float yRotation = 0.0f;

        private bool cantPress = false;

        private float sceneDistance = 1200.0f;

        private float rastojanjeKamere = 2000;

        private float faktorSkaliranjaBureta = 1.0f;

        private float lightR = 1.0f;
        private float lightG = 0.0f;
        private float lightB = 0.0f;

        private float pozicijaReflektora = 450f;

        private float bureX = 450;
        private float bureY = 280;

        private String[] m_bitmapText = { "Predmet:   Racunarska grafika",
                                          "Sk.god:     2016./17.",
                                          "Ime:           Zlatan" ,
                                          "Prezime:   Precanica",
                                          "Sifra zad:   13.2"};
        //// Parametri za animaciju
        private float ugaoDrzaca = 0;

        private float rotirajBure = 0;

        private DispatcherTimer timer1;

        private LookAtCamera lookAtCam;

        private enum TextureObjects { Beton, Metal, Drvo };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private string[] m_textureFiles = { "..//..//images//beton.jpg", "..//..//images//metal.jpg", "..//..//images//drvo.jpg" };

        private uint[] m_textures = null;
        
        // sirina kontrole u pikselima
        // kada se prozor siri da se siri i scena
        private int width;

        private int height;

        public AssimpScene Scene
        {
            get { return scene; }
            set { scene = value; }
        }

        public float RotationX
        {
            get { return xRotation; }
            set { xRotation = value; }
        }

        public float RotationY
        {
            get { return yRotation; }
            set { yRotation = value; }
        }

        public float SceneDistance
        {
            get { return sceneDistance; }
            set { sceneDistance = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public float FaktorBuretaSkal
        {
            get { return faktorSkaliranjaBureta; }
            set { faktorSkaliranjaBureta = value; }
        }

        public float LightRED
        {
            get { return lightR; }
            set { lightR = value; }
        }

        public float LightGREEN
        {
            get { return lightG; }
            set { lightG = value; }
        }

        public float LightBLUE
        {
            get { return lightB; }
            set { lightB = value; }
        }

        public float PozicijaReflektora
        {
            get { return pozicijaReflektora; }
            set { pozicijaReflektora = value; }
        }

        public bool Press
        {
            get { return cantPress; }
            set { cantPress = value; }
        }

        public float Zoom
        {
            get { return rastojanjeKamere; }
            set { rastojanjeKamere = value; }
        }

        ~World()
        {
            this.Dispose(false);
        }
        //TODO 11 Animacija
        public void PlayAnimation()
        {
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(200);
            timer1.Tick += new EventHandler(UpdateAnimation);
            timer1.Start();
        }

        public int i = 0;
        public void UpdateAnimation(object sender, EventArgs e)
        {
            cantPress = true;

            if (ugaoDrzaca < 90)
            {
                ugaoDrzaca += 20;
            }
            

            if (ugaoDrzaca > 20 && bureX>200)
            {
                rotirajBure += 90;
                bureX -= 50;
                bureY -= 5;
            }
            else if (ugaoDrzaca > 20 && bureX >= -460 && bureX <= 200)
            {
                rotirajBure += 90;
                bureX -= 50;
                bureY -= 15;
            }
            else if (ugaoDrzaca > 20 && bureX <= -460)
            {
                rotirajBure += 90;
                bureX -= 10;
                bureY -= 60;
            }

            if(bureY <= -120){
                StopAnimation();
            }

        }

        public void StopAnimation()
        {
            cantPress = false;
            timer1.Stop();
            ugaoDrzaca = 0;
            bureX = 450;
            bureY = 280;
        }

        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);  //pozadina
            gl.ShadeModel(OpenGL.GL_FLAT);      // Model sencenja na flat (konstantno)
            gl.Enable(OpenGL.GL_DEPTH_TEST);    //ukljucivanje testiranja dubine
            gl.Enable(OpenGL.GL_CULL_FACE);     //skrivanje nevidjivih povrsina

            //teksture za cube disk i quad, način stapanja teksture sa materijalom postaviti da bude GL_ADD. 
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            //tackastiIzvor(gl);

            //reflektorskiIzvor(gl);

            //TODO 1 Uključiti color tracking mehanizam i podesiti da se pozivom metode glColor definiše ambijentalna i difuzna komponenta materijala.
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //TODO 3 Ucitavanje textura
            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }

            //TODO 6 Kamera
            lookAtCam = new LookAtCamera();
            lookAtCam.Position = new Vertex(0, 180, 2000);
            lookAtCam.Target = new Vertex(0f, 150f, 0);
            lookAtCam.UpVector = new Vertex(0f, 1f, 0f);
            lookAtCam.AspectRatio = width / height;
            lookAtCam.Near = 1.0f;
            lookAtCam.Far = 20000f;

            gl.LookAt(0.0f, 180.0f, 2000,
                     0.0f, 150.0f, 0,
                     0.0f, 1.0f, 0.0f);

            ////gl.Perspective(60f, (double)width / height, 1.0f, 20000f);

            scene.LoadScene();
            scene.Initialize();
        }

        public void Draw(OpenGL gl)
        {
            tackastiIzvor(gl);

            reflektorskiIzvor(gl);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            lookAtCam.Position = new Vertex(0, 180, rastojanjeKamere);
            

            //gl.Viewport(width / 2, 0, width / 2, height / 2);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            ////gl.Ortho(0f,20.0f,0f,20f,-1.0f,1.0f);
            //gl.Ortho2D(-15.0f, 13f, -4.1f, 12f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.PushMatrix();
            //gl.Color(1.0f, 1.0f, 0f);
            //gl.DrawText3D("Arial Bold", 25f, 0f, 0f, "Predmet: Racunarska grafika");
            //gl.PopMatrix();

            //gl.Viewport(width / 2, 0, width / 2, height / 2);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            ////gl.Ortho(0f,20.0f,0f,20f,-1.0f,1.0f);
            //gl.Ortho2D(-15.0f, 13f, -3.1f, 13f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.PushMatrix();
            //gl.Color(1.0f, 1.0f, 0f);
            //gl.DrawText3D("Arial Bold", 14f, 0f, 0f, "Sk.god: 2016/17");
            //gl.PopMatrix();

            //gl.Viewport(width / 2, 0, width / 2, height / 2);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            ////gl.Ortho(0f,20.0f,0f,20f,-1.0f,1.0f);
            //gl.Ortho2D(-15.0f, 13f, -2.1f, 14f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.PushMatrix();
            //gl.Color(1.0f, 1.0f, 0f);
            //gl.DrawText3D("Arial Bold", 14f, 0f, 0f, "Ime: Zlatan");
            //gl.PopMatrix();

            //gl.Viewport(width / 2, 0, width / 2, height / 2);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            ////gl.Ortho(0f,20.0f,0f,20f,-1.0f,1.0f);
            //gl.Ortho2D(-15f, 13f, -1.1f, 15f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.PushMatrix();
            //gl.Color(1.0f, 1.0f, 0f);
            //gl.DrawText3D("Arial Bold", 14f, 0f, 0f, "Prezime: Precanica");
            //gl.PopMatrix();

            //gl.Viewport(width / 2, 0, width / 2, height / 2);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            ////gl.Ortho(0f,20.0f,0f,20f,-1.0f,1.0f);
            //gl.Ortho2D(-15.0f, 13f, -0.1f, 16f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.PushMatrix();
            //gl.Color(1.0f, 1.0f, 0f);
            //gl.DrawText3D("Arial Bold", 14f, 0f, 0f, "Sifra zad: 13.2");
            //gl.PopMatrix();
            ////iznad tekst
            /*
            gl.Enable(OpenGL.GL_CULL_FACE);
            //barrel
            gl.LoadIdentity();

            gl.PushMatrix();

            gl.Translate(0.0f, 0.0f, -sceneDistance);
            gl.Rotate(xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(yRotation, 0.0f, 1.0f, 0.0f);

            gl.PushMatrix();

            gl.Color(1f, 1f, 1f, 1f);

            tackastiIzvor(gl);

            gl.PushMatrix();

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Drvo]);
            gl.Color(0.5f, 0.2f, 0.0f);
            Cube kocka = new Cube();
            gl.Translate(400, 100, 0);
            gl.Scale(100, 100, 100);
            kocka.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.Translate(0, 180, -110);
            gl.Rotate(90, 1.0f, 0.0f, 0.0f);
            scene.Draw();

            gl.PopMatrix();

            gl.PushMatrix();

            gl.Disable(OpenGL.GL_CULL_FACE);

            gl.Translate(-500.0f, 1.0f, 0.0f);
            gl.Rotate(-90, 1.0f, 0.0f, 0.0f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);

            gl.Color(0, 0, 0);

            Disk obod = new Disk();
            obod.InnerRadius =180f;
            obod.OuterRadius = 200.0f;
            obod.Slices = 100;
            obod.Loops = 100;

            obod.CreateInContext(gl);
            obod.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            Disk disk = new Disk();
            disk.TextureCoords = true;
            disk.InnerRadius = 0.0f;
            disk.OuterRadius = 180.0f;
            disk.Slices = 100;
            disk.Loops = 100;

            disk.CreateInContext(gl);
            disk.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Beton]);
            gl.Color(0.2f,0.2f,0.2f);
            //podloga koja predstavlja more, uvek idu iste dimenzije, i ide -+, ++, +-, -- za x i z
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0f, 1f, 0f);
            //gl.Color(0.0f, 0.7f, 1.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-800, -0.0, 800);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(800, -0.0, 800);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(800, -0.0, -800);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-800, -0.0, -800);

            gl.End();

            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.PopMatrix();

            gl.PopMatrix();

            gl.Flush();
            */
            //ELEMENTI

            gl.Viewport(0, 0, width, height);
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.LoadIdentity();
            //gl.Perspective(60f, (double)width / height, 1.0f, 20000f);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            
            gl.PushMatrix();

            lookAtCam.Project(gl);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //gl.Translate(0.0f, 0.0f, -sceneDistance);
            gl.Rotate(xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(yRotation, 0.0f, 1.0f, 0.0f);

            gl.PushMatrix();

            //iscrtavanje Podloge
            gl.PushMatrix();
            //TODO 5 Texture podloge
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Beton]);
            gl.Color(0.1,0.1,0.1);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-800.0f, 0.0f, 800.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(800.0f, 0.0f, 800.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(800.0f, 0.0f, -800.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-800.0f, 0.0f, -800.0f);
            gl.End();

            gl.PopMatrix();

            //iscrtavanje Bureta
            gl.PushMatrix();
            //TODO 10 Način stapanja teksture sa materijalom za model bureta postaviti na GL_ADD
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Drvo]);
            //Način stapanja teksture sa materijalom za modelburetapostaviti na GL_ADD.
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.Translate(bureX,bureY, -110);
            gl.Rotate(90,1.0f,0.0f,0.0f);
            gl.Rotate(rotirajBure, 0.0f, 1.0f, 0.0f);
            gl.Scale(1.0f, faktorSkaliranjaBureta, 1.0f);
            gl.Color(0.5f, 0.2f, 0.0f);
            scene.Draw();
            gl.PopMatrix();

            //iscrtavanje Kocke
            //tekstura cube
            

            gl.PushMatrix();
            //TODO 4 Texture
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Drvo]);
            gl.Color(0.2f, 0.1f, 0.0f);
            gl.Translate(420, 100, 30);
            Cube kocka = new Cube();
            gl.Scale(150,100,140);
            kocka.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //padina
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Drvo]);
            gl.Color(0.2f, 0.1f, 0.0f);
            gl.Translate(-35, 95, 0);
            gl.Rotate(15, 0.0f, 0.0f, 1.0f);
            Cube letva = new Cube();
            gl.Scale(350, 10, 80);
            kocka.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //drzac
            gl.PushMatrix();
            gl.Color(0f, 0f, 0f);
            gl.Translate(360, 180, 30);
            gl.Rotate(ugaoDrzaca,0.0,0.0,1.0);
            Cube drzac = new Cube();
            gl.Scale(5, 80, 100);
            drzac.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //iscrtavanje Diska
            gl.PushMatrix();

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);
            gl.Color(0.0f, 0.0f, 0.0f);
            

            gl.Translate(-500.0f, 1.0f, 0.0f);
            gl.Rotate(-90, 1.0f, 0.0f, 0.0f);

            Disk disk = new Disk();
            disk.TextureCoords = true;
            disk.InnerRadius = 0.0f;
            disk.OuterRadius = 200.0f;
            disk.Slices = 100;
            disk.Loops = 100;

            disk.CreateInContext(gl);
            disk.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);


            gl.PopMatrix();

            gl.PushMatrix();
            
            gl.Translate(-500.0f, 1.0f, 0.0f);
            gl.Rotate(-90, 1.0f, 0.0f, 0.0f);

            Disk obod = new Disk();
            obod.InnerRadius = 200f;
            obod.OuterRadius = 210.0f;
            obod.Slices = 100;
            obod.Loops = 100;

            obod.CreateInContext(gl);
            obod.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PopMatrix(); //iscrtani elementi
            for (int i = 0; i < m_bitmapText.Length; i++)
            {
                gl.DrawText(800, 100 - i * 15, 10.0f, 10.0f, 0.0f, "Arial Bold", 14, m_bitmapText[i]);
            }
            gl.PopMatrix(); //rotacije

           

            gl.Flush();
        }

        //TODO 2 Definisati tačkasti svetlosni izvorbele boje
        public void tackastiIzvor(OpenGL gl)
        {
          
            float[] light0pos = new float[] { 800, 800, 0, 1.0f };
            float[] light0ambient = new float[] { 1f, 1f, 1f, 1.0f }; // posednji parametar: da li je tackasti, direkcioni-> TACKASTI: 1
            float[] light0diffuse = new float[] { 1f, 1f, 1f, 1.0f }; 

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f); // Pomeri se u tacku gde je svetlo pozicionirano

            gl.Enable(OpenGL.GL_NORMALIZE);

            //ukljucivanje ambijentalne i difuzne svetlosti materijala
            //gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            //gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //gl.ShadeModel(OpenGL.GL_SMOOTH);
            
           // float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            //bela spekularna komponenta materijala sa jakim odsjajem
            //gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, light0specular); 
            //gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);
        }

        //TODO 9 Reflektor
        public void reflektorskiIzvor(OpenGL gl)
        {
            float[] ambijentalnaKomponenta = { lightR, lightG, lightB, 1.0f };
            float[] difuznaKomponenta = { lightR, lightG, lightB, 1.0f };
            //float[] spekularnaKomponenta = { 1f, 0.0f, 0.0f, 1.0f };
            float[] smer = { 0.0f, -1.0f, 0.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, spekularnaKomponenta);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30f);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Enable(OpenGL.GL_NORMALIZE);


            float[] pozicija = { pozicijaReflektora, 800,0, 1.0f }; //gl.Translate(450,280, -110);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pozicija);
        }

        // projekcija i viewport
        public void Resize(OpenGL gl, int width, int height)
        {
            this.width = width;
            this.height = height;
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                
        }


        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.scene = new AssimpScene(scenePath, sceneFileName,gl);
            this.width = width;
            this.height = height;

            m_textures = new uint[m_textureCount];
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                scene.Dispose();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
