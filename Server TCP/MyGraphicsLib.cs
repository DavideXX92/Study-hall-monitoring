using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using Emgu.CV.Util;

using DB;
using SpotDao;
using TableDAO;
using Emgu.CV.UI;
namespace EMGU_Program {
    public class Blob {
        public VectorOfPoint contour;
        public Rectangle boundingRect;
        public Point centerPosition;
        public double dblDiagonalSize;
        public double dblAspectRatio;

        public Blob(VectorOfPoint _contour) {
            contour = _contour;
            boundingRect = createRect(contour);
            CvInvoke.Rectangle(contour, boundingRect, new MCvScalar(0, 200, 0));
            centerPosition.X = (boundingRect.X + boundingRect.X + boundingRect.Width) / 2;
            centerPosition.Y = (boundingRect.Y + boundingRect.Y + boundingRect.Height) / 2;
            dblDiagonalSize = Math.Sqrt(Math.Pow(boundingRect.Width, 2) + Math.Pow(boundingRect.Height, 2));
            dblAspectRatio = (float)boundingRect.Width / (float)boundingRect.Height;
        }

        private Rectangle createRect(VectorOfPoint contour) {
            int x, y, width, height;
            int max_x = -1, max_y = -1;
            int min_x = 1000, min_y = 1000;
            for (int i = 0; i < contour.Size; i++) {
                Point p = contour[i];
                if (p.X > max_x)
                    max_x = p.X;
                if (p.X < min_x)
                    min_x = p.X;
                if (p.Y > max_y)
                    max_y = p.Y;
                if (p.Y < min_y)
                    min_y = p.Y;
            }
            x = min_x;
            y = min_y;
            width = max_x - min_x + 1;
            height = max_y - min_y + 1;
            Rectangle rect = new Rectangle(x, y, width, height);
            return rect;
        }
    }

    public class MyGraphicsLib {
        public MCvScalar SCALAR_BLACK = new MCvScalar(0, 0, 0);
        public MCvScalar SCALAR_WHITE = new MCvScalar(255, 255, 255);
        public MCvScalar SCALAR_RED = new MCvScalar(255, 0, 0);
        public MCvScalar SCALAR_GREEN = new MCvScalar(0, 200, 0);
        public MCvScalar SCALAR_BLUE = new MCvScalar(0, 0, 255);

        public MyGraphicsLib() {
        }

        public void compute(String roomEmptyPath, byte[] room, State state, List<Spot> spots, List<Table> tables) {

            int count = 0, countAttuale = 0, NumPosti = 0, new_spot = 0;
            int[,] PostiOccupati = new int[500, 2];
            int[] PostiNuovi = new int[500];

            if (state == State.Monitoring)
                foreach (Spot tmp in spots)
                    tmp.IsFree = true;
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////      RICEVO(IMGvuota,IMGFRAMEx,Stato)
            Mat imgFrame1 = CvInvoke.Imread(roomEmptyPath, LoadImageType.Color);  //frame Aula studio vuota
            Mat imgFrame2 = new Mat();
            CvInvoke.Imdecode(room, LoadImageType.Color, imgFrame2);              //frame Aula studio studenti dall'array di byte
            //imgFrame2 = CvInvoke.Imread("C:/Users/Davide/Desktop/immagini gadgeteer/3.bmp", LoadImageType.Color);     //frame Aula studio studenti dall'immagine
            // 1-> LEARNING(Apprendi Posizione posti e tavoli
            // 0-> Ricerca posti liberi/occupati
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            NumPosti = spots.Count;

            if (state == State.Learning) {                                   //LEARNING
                if (tables.Count == 0)
                    SearchTable(roomEmptyPath, tables);

                for (int i = 0; i < 500; i++)
                    PostiNuovi[i] = 0;
            }


            Mat imgFrame1Copy = imgFrame1.Clone();
            Mat imgFrame2Copy = imgFrame2.Clone();

            //Conversione scala di grigi
            CvInvoke.CvtColor(imgFrame1Copy, imgFrame1Copy, ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(imgFrame2Copy, imgFrame2Copy, ColorConversion.Bgr2Gray);

            //Filtro gaussiano
            CvInvoke.GaussianBlur(imgFrame1Copy, imgFrame1Copy, new System.Drawing.Size(5, 5), 0);
            CvInvoke.GaussianBlur(imgFrame2Copy, imgFrame2Copy, new System.Drawing.Size(5, 5), 0);
            CvInvoke.GaussianBlur(imgFrame2Copy, imgFrame2Copy, new System.Drawing.Size(5, 5), 0);
            CvInvoke.GaussianBlur(imgFrame2Copy, imgFrame2Copy, new System.Drawing.Size(5, 5), 0);

            Mat imgDifference = new Mat();
            CvInvoke.AbsDiff(imgFrame1Copy, imgFrame2Copy, imgDifference);

            //Prendo in esame la scala di grigi in un intervallo specifico
            Mat imgThresh = new Mat();
            CvInvoke.Threshold(imgDifference, imgThresh, 80, 160, ThresholdType.Binary);
            // ImageViewer.Show(imgThresh, "Threshold Window");

            Mat structuringElement3x3 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            Mat structuringElement5x5 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(5, 5), new System.Drawing.Point(-1, -1));
            Mat structuringElement7x7 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(7, 7), new System.Drawing.Point(-1, -1));
            Mat structuringElement9x9 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(9, 9), new System.Drawing.Point(-1, -1));

            //Erosioni e dilatamenti dei pixels per rifinire la ricerca
            /* CvInvoke.Erode(imgThresh, imgThresh, structuringElement9x9, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
             */
            CvInvoke.Erode(imgThresh, imgThresh, structuringElement5x5, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement5x5, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement5x5, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement5x5, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            //  CvInvoke.Dilate(imgThresh, imgThresh, structuringElement3x3, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);

            Mat imgThreshCopy = imgThresh.Clone();
            //ImageViewer.Show(imgThreshCopy, "imgDifference Window");
            //APPLICO UN CONTORNO BEN DEFINITO AGLI ELEMENTI TROVATI
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(imgThreshCopy, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
            Mat imgContours = new Mat(imgThresh.Size, DepthType.Cv8U, 3);
            CvInvoke.DrawContours(imgContours, contours, -1, SCALAR_WHITE, -1);

            VectorOfVectorOfPoint convexHulls = new VectorOfVectorOfPoint(contours.Size);
            for (int i = 0; i < contours.Size; i++)
                CvInvoke.ConvexHull(contours[i], convexHulls[i]);

            List<Blob> blobs = new List<Blob>();
            //VERIFICO CHE I CONTORNI ABBIAMO DEI REQUISITI BEN SPECIFICI
            for (int i = 0; i < convexHulls.Size; i++) {
                VectorOfPoint convexHull = convexHulls[i];
                Blob possibleBlob = new Blob(convexHull);
                int area = possibleBlob.boundingRect.Width * possibleBlob.boundingRect.Height;

                if (area > 100 &&
                    possibleBlob.dblAspectRatio >= 0.2 &&
                    possibleBlob.dblAspectRatio <= 1.25 &&
                    possibleBlob.boundingRect.Width > 15 &&
                    possibleBlob.boundingRect.Height > 15 &&
                    possibleBlob.dblDiagonalSize > 20.0
                 ) {
                    blobs.Add(possibleBlob);
                    countAttuale++;
                }
            }

            foreach (Blob blob in blobs)
                convexHulls.Push(blob.contour);

            // get another copy of frame 2 since we changed the previous frame 2 copy in the processing above
            imgFrame2Copy = imgFrame2.Clone();

            String coordString = null;
            foreach (Blob blob in blobs) {                                                                               // for each blob
                CvInvoke.Rectangle(imgFrame2Copy, blob.boundingRect, SCALAR_RED, 2);                 // draw a red box around the blob
                CvInvoke.Circle(imgFrame2Copy, blob.centerPosition, 5, SCALAR_GREEN, -1);   // draw a filled-in green circle at the center

                //printf("posizione: x:%d y:%d \n", blob.centerPosition.x, blob.centerPosition.y);
                int coord_x = blob.centerPosition.X;
                int coord_y = blob.centerPosition.Y;

                Console.WriteLine("x: " + coord_x + " y: " + coord_y);
                coordString += "x: " + coord_x + " y: " + coord_y + "\n";
                Console.WriteLine("posizione: x:" + coord_x + " y:" + coord_y + "\n");

                for (int i = 0; i < count + NumPosti; i++)				   //ricerca per affinare il DB con i nuovi dati
                    foreach (Spot tmp in spots) {
                        if (((coord_x <= tmp.X + 38 && coord_x >= tmp.X - 38)) && ((coord_y <= tmp.Y + 90 && coord_y >= tmp.Y - 90))) {
                            // Console.WriteLine("posizione: x:" + tmp.X + " y:" + tmp.Y + "\n");
                            if (state == State.Monitoring)
                                tmp.IsFree = false;
                            if (state == State.Learning)
                                tmp.Count++;
                            new_spot = 1;
                        }
                    }

                if (new_spot == 0 && state == State.Learning) {
                    count++;
                    spots.Add(new Spot(coord_x, coord_y));
                    Console.WriteLine("SCOPERTO POSTO -> " + count + " Coordinate x: " + coord_x + "  y: " + coord_y + "\n");             //INVIARE A DB
                }
                new_spot = 0;
            }
        }

        public void SearchTable(String roomEmptyPath, List<Table> tables) {

            Mat imgFrame1 = CvInvoke.Imread(roomEmptyPath, LoadImageType.Color);
            Mat imgFrame1Copy = imgFrame1.Clone();

            CvInvoke.CvtColor(imgFrame1Copy, imgFrame1Copy, ColorConversion.Bgr2Gray);

            CvInvoke.GaussianBlur(imgFrame1Copy, imgFrame1Copy, new System.Drawing.Size(5, 5), 0);

            Mat imgThresh = new Mat();
            CvInvoke.Threshold(imgFrame1Copy, imgThresh, 100, 255, ThresholdType.Binary);



            Mat imgBlack = new Mat();
            CvInvoke.Threshold(imgFrame1Copy, imgBlack, 255, 255, ThresholdType.Binary);

            Mat structuringElement3x3 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            Mat structuringElement5x5 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(5, 5), new System.Drawing.Point(-1, -1));
            Mat structuringElement7x7 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(7, 7), new System.Drawing.Point(-1, -1));
            Mat structuringElement9x9 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(9, 9), new System.Drawing.Point(-1, -1));

            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement9x9, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement9x9, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement9x9, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);
            CvInvoke.Dilate(imgThresh, imgThresh, structuringElement9x9, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);

            CvInvoke.Erode(imgThresh, imgThresh, structuringElement5x5, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);


            Mat imgThreshCopy = imgThresh.Clone();

            CvInvoke.BitwiseNot(imgThreshCopy, imgThreshCopy);
            CvInvoke.Erode(imgThreshCopy, imgThreshCopy, structuringElement3x3, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, SCALAR_BLACK);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            //  ImageViewer.Show(imgThreshCopy, "Test Window");
            CvInvoke.FindContours(imgThreshCopy, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            //  ImageViewer.Show(imgThreshCopy, "Test Window");
            Mat imgContours = new Mat(imgThresh.Size, DepthType.Cv8U, 3);
            CvInvoke.DrawContours(imgContours, contours, -1, SCALAR_WHITE, -1);

            VectorOfVectorOfPoint convexHulls = new VectorOfVectorOfPoint(contours.Size);
            for (int i = 0; i < contours.Size; i++)
                CvInvoke.ConvexHull(contours[i], convexHulls[i]);

            List<Blob> blobs = new List<Blob>();

            for (int i = 0; i < convexHulls.Size; i++) {
                VectorOfPoint convexHull = convexHulls[i];
                Blob possibleBlob = new Blob(convexHull);
                blobs.Add(possibleBlob);
            }

            Mat imgConvexHulls = new Mat(imgThresh.Size, DepthType.Cv8U, 3);
            convexHulls.Clear();

            foreach (Blob blob in blobs)
                convexHulls.Push(blob.contour);

            CvInvoke.DrawContours(imgConvexHulls, convexHulls, -1, SCALAR_WHITE, -1);

            int x, y, w, h;
            foreach (Blob blob in blobs) {
                CvInvoke.Rectangle(imgConvexHulls, blob.boundingRect, SCALAR_RED, 3);

                x = blob.boundingRect.X;
                y = blob.boundingRect.Y;
                w = blob.boundingRect.Width;
                h = blob.boundingRect.Height;
                tables.Add(new Table(x, y, h, w));
                //INFORMAZIONE SUI TAVOLI DA INVIARE AL DB
                Console.WriteLine("tavolo: x: " + x + " y: " + y + " w: " + w + " h: " + h);
            }
        }

    }

}
