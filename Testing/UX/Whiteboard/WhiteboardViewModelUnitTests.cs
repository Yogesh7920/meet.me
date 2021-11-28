/**
 * Owned By: Arpan Tripathi
 * Created By: Arpan Tripathi
 * Date Created: 25/10/2021
 * Date Modified: 28/11/2021
**/

using Client;
using NUnit.Framework;
using Whiteboard;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Testing.UX.Whiteboard
{
    [TestFixture , Apartment(System.Threading.ApartmentState.STA)]
    public class WhiteboardViewModelUnitTests
    {
        private WhiteBoardViewModel _viewModel;
        private Canvas _globCanvas;
        private IWhiteBoardOperationHandler _WBOps;
        
        //some colors for customization 
        private string Black = "#000000";
        private string White = "#FFFFFF";
        private string Red = "#FF0000";
        private string Green = "#00FF00";
        private string Blue = "#0000FF";

        [SetUp]
        public void SetUp()
        {
            // Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            _globCanvas = new Canvas() { Height = 300, Width = 500 };
            _viewModel = new WhiteBoardViewModel(_globCanvas, testing:true);
        }

        [Test]
        public void testSelection1()
        {
            // Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            string uid = "auniqueidentifier";
            Shape sh = new System.Windows.Shapes.Rectangle() { Height = 10, Width = 30, Uid = uid };

            Random r = new Random();
            int left = r.Next(1, (int)_globCanvas.Width - ((int)sh.Width + 1));
            int top = r.Next(1, (int)_globCanvas.Height - ((int)sh.Height + 1));

            Canvas.SetLeft(sh, left);
            Canvas.SetTop(sh, top);

            _globCanvas.Children.Add(sh);

            _globCanvas = _viewModel.shapeManager.SelectShape(_globCanvas, sh, _WBOps);


            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            _globCanvas = _viewModel.shapeManager.UnselectAllBB(_globCanvas, _WBOps);
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 0);

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testDelete1()
        {
            string uid = "auniqueidentifier";
            Shape sh = new System.Windows.Shapes.Rectangle() { Height = 10, Width = 30, Uid = uid };

            Random r = new Random();
            int left = r.Next(1, (int)_globCanvas.Width - ((int)sh.Width + 1));
            int top = r.Next(1, (int)_globCanvas.Height - ((int)sh.Height + 1));

            Canvas.SetLeft(sh, left);
            Canvas.SetTop(sh, top);

            _globCanvas.Children.Add(sh);

            _globCanvas = _viewModel.shapeManager.SelectShape(_globCanvas, sh, _WBOps);

            //Check if selection is performed correctly 
            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            //Delete Selected Shapes 
            _globCanvas = _viewModel.shapeManager.DeleteSelectedShapes(_globCanvas, _WBOps);

            //Check if selected shape is empty 
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 0);
            Assert.AreEqual(_globCanvas.Children.Count, 0);

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testDuplicate1()
        {
            string uid = "auniqueidentifier";
            Shape sh = new System.Windows.Shapes.Rectangle() { Height = 10, Width = 30, Uid = uid };

            Random r = new Random();
            int left = r.Next(1, (int)_globCanvas.Width - ((int)sh.Width + 1));
            int top = r.Next(1, (int)_globCanvas.Height - ((int)sh.Height + 1));

            Canvas.SetLeft(sh, left);
            Canvas.SetTop(sh, top);

            _globCanvas.Children.Add(sh);

            _globCanvas = _viewModel.shapeManager.SelectShape(_globCanvas, sh, _WBOps);

            //Check if selection is performed correctly 
            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            //Delete Selected Shapes 
            _globCanvas = _viewModel.shapeManager.DuplicateShape(_globCanvas, _WBOps);

            //Check if selected shape contains only the duplicated shape 
            Assert.AreEqual(_globCanvas.Children.Count, 2); 
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);
            Assert.AreNotEqual(_viewModel.shapeManager.selectedShapes[0], uid); 

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testCustomize1()
        {
            string uid = "auniqueidentifier";
            Shape sh = new System.Windows.Shapes.Rectangle() { Height = 10, Width = 30, Uid = uid };

            Random r = new Random();
            int left = r.Next(1, (int)_globCanvas.Width - ((int)sh.Width + 1));
            int top = r.Next(1, (int)_globCanvas.Height - ((int)sh.Height + 1));

            Canvas.SetLeft(sh, left);
            Canvas.SetTop(sh, top);

            _globCanvas.Children.Add(sh);

            _globCanvas = _viewModel.shapeManager.SelectShape(_globCanvas, sh, _WBOps);

            //Check if selection is performed correctly 
            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            //Check for Stroke Color
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Stroke", Black);
            Assert.AreEqual(((SolidColorBrush)sh.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Black))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Stroke", White);
            Assert.AreEqual(((SolidColorBrush)sh.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(White))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Stroke", Red);
            Assert.AreEqual(((SolidColorBrush)sh.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Red))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Stroke", Blue);
            Assert.AreEqual(((SolidColorBrush)sh.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Blue))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Stroke", Green);
            Assert.AreEqual(((SolidColorBrush)sh.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Green))).Color);

            //Check for Fill Color 
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Fill", Black);
            Assert.AreEqual(((SolidColorBrush)sh.Fill).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Black))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Fill", White);
            Assert.AreEqual(((SolidColorBrush)sh.Fill).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(White))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Fill", Red);
            Assert.AreEqual(((SolidColorBrush)sh.Fill).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Red))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Fill", Blue);
            Assert.AreEqual(((SolidColorBrush)sh.Fill).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Blue))).Color);
            _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "Fill", Green);
            Assert.AreEqual(((SolidColorBrush)sh.Fill).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Green))).Color);

            //Check for Stroke Thickness 
            for (int t = 1; t <= 10; t++)
            {
                _globCanvas = _viewModel.shapeManager.CustomizeShape(_globCanvas, _WBOps, property: "StrokeThickness", Black, thickness:t);
                Assert.AreEqual(sh.StrokeThickness, t); 
            }

            _globCanvas = _viewModel.shapeManager.UnselectAllBB(_globCanvas, _WBOps);

            //Check if selected shape is empty 
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 0);

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testCreate1()
        {
            //Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            Random r = new Random();
            int twidth = 30;
            int theight = 10;

            //Mouse points 
            Point strt;
            Point end;

            strt.X = r.Next(1, (int)_globCanvas.Width - (twidth + 1));
            strt.Y = r.Next(1, (int)_globCanvas.Height - (theight + 1));

            //set mouse down i.e. starting point  
            end = strt;
            //mouse move s
            for (int i = 1; i <= twidth; i++)
            {
                for (int j = 1; j <= theight; j++)
                {
                    end.X = strt.X + i;
                    end.Y = strt.Y + j;
                    _globCanvas = _viewModel.shapeManager.CreateShape(_globCanvas, _WBOps, WhiteBoardViewModel.WBTools.NewRectangle, strt, end, shapeComp: false);
                }
            }

            //mouse up 
            _globCanvas = _viewModel.shapeManager.CreateShape(_globCanvas, _WBOps, WhiteBoardViewModel.WBTools.NewRectangle, strt, end, shapeComp: true);

            //chck if shape created is selected 
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            //get the created shape 
            string shUID = _viewModel.shapeManager.selectedShapes[0];

            IEnumerable<UIElement> iterat = _globCanvas.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

            //Check only one shape is selected 
            Assert.AreEqual(iterat.Count(), 1);

            Shape sh = (Shape)_globCanvas.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

            var x = Math.Min(end.X, strt.X);
            var y = Math.Min(end.Y, strt.Y);

            var w = Math.Max(end.X, strt.X) - x;
            var h = Math.Max(end.Y, strt.Y) - y;

            int final_left = (int)Canvas.GetLeft(sh);
            int final_top = (int)Canvas.GetTop(sh);

            int final_width = (int)sh.Width;
            int final_height = (int)sh.Height;

            Assert.AreEqual(x, final_left);
            Assert.AreEqual(y, final_top);
            Assert.AreEqual(w, final_width);
            Assert.AreEqual(h, final_height);
            Assert.AreEqual(twidth, final_width);
            Assert.AreEqual(theight, final_height);

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testMove1()
        {
            //Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            string uid = "auniqueidentifier";
            Shape sh = new System.Windows.Shapes.Rectangle() { Height = 10, Width = 30, Uid = uid };
            
            Random r = new Random();
            int left = r.Next(240, 250);
            int top = r.Next(140, 150);

            Canvas.SetLeft(sh, left);
            Canvas.SetTop(sh, top);
            
            _globCanvas.Children.Add(sh);

            //horizontal Displacement 
            Point strt;
            Point temp; 
            Point end;

            strt.X = r.Next((int)Canvas.GetLeft(sh), (int)(Canvas.GetLeft(sh) + sh.Width));
            strt.Y = r.Next((int)Canvas.GetTop(sh), (int)(Canvas.GetTop(sh) + sh.Height));

            //mouse down 
            //select the shape 
            _globCanvas = _viewModel.shapeManager.SelectShape(_globCanvas, sh, _WBOps);

            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            //move the shape 
            temp = strt;
            end = strt; 
            //mouse move 
            for (int i = 1; i <= 10; i++)
            {
                
                if(end.X < _globCanvas.Width && end.X > 0)
                {
                    end.X = end.X + 1;
                }
                else
                {
                    break;
                }

                if(end.Y < _globCanvas.Height && end.Y > 0) 
                { 
                    end.Y = end.Y + 1;
                }
                else
                {
                    break;
                }

                _globCanvas = _viewModel.shapeManager.MoveShape(_globCanvas, _WBOps, temp, end, sh, false);
                temp = end; 
            }

            //mouse up 
            _globCanvas = _viewModel.shapeManager.MoveShape(_globCanvas, _WBOps, temp, end, sh, true);

            double dX = end.X - strt.X;
            double dY = end.Y - strt.Y;
            double distance = Math.Sqrt(dX * dX + dY * dY);

            int final_left = (int)Canvas.GetLeft(sh);
            int final_top = (int)Canvas.GetTop(sh);

            double shdX = final_left - left;
            double shdY = final_top - top;
            double shp_distance = Math.Sqrt(shdX * shdX + shdY * shdY);

            Assert.AreEqual(distance, shp_distance);

            //Shape should be still selected 
            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testCreatePolyline1()
        {
            //Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            Random r = new Random();
            //Mouse points 
            Point strt;
            Point end;
            PointCollection drawingPoints = new PointCollection();

            strt.X = r.Next(240, 250);
            strt.Y = r.Next(140, 150);
            drawingPoints.Add(strt);

            _viewModel.freeHand.SetColor(Red);
            _viewModel.freeHand.SetThickness(2);
            _globCanvas = _viewModel.freeHand.DrawPolyline(_globCanvas, _WBOps, strt, true, false, false);

            //set mouse down i.e. starting point  
            end = strt;
            
            //mouse move 
            for (int i = 1; i <= 20; i++)
            {
                if (end.X < _globCanvas.Width && end.X > 0)
                {
                    end.X = end.X + 1;
                }
                else
                {
                    break;
                }

                if (end.Y < _globCanvas.Height && end.Y > 0)
                {
                    end.Y = end.Y + 1;
                }
                else
                {
                    break;
                }

                drawingPoints.Add(end);
                _globCanvas = _viewModel.freeHand.DrawPolyline(_globCanvas, _WBOps, end, false, false, false);
            }

            //mouse up 
            _globCanvas = _viewModel.freeHand.DrawPolyline(_globCanvas, _WBOps, end, false, false, true);

            //get the created Line

            Shape sh = (Shape)_globCanvas.Children.OfType<UIElement>().Where(x => x.Uid == "auniquepoly").ToList()[0];
            System.Windows.Shapes.Polyline line = ((System.Windows.Shapes.Polyline)sh);

            Assert.AreEqual(line.StrokeThickness, 2);
            Assert.AreEqual(((SolidColorBrush)line.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Red))).Color);

            Assert.AreEqual(drawingPoints, line.Points); 

            _globCanvas.Children.Clear();
        }

        [Test]
        public void testDeletePolyline1()
        {
            //Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            Random r = new Random();
            //Mouse points 
            Point strt;
            Point end;
            PointCollection drawingPoints = new PointCollection();

            strt.X = r.Next(1, (int)_globCanvas.Width - 1);
            strt.Y = r.Next(1, (int)_globCanvas.Height - 1);
            drawingPoints.Add(strt);

            //mouse move 
            for (int i = 1; i <= 20; i++)
            {
                end.X = strt.X + i;
                end.Y = strt.Y + i;
                drawingPoints.Add(end);
            }

            // Create a polyline  
            string shUid = "auniquepoly";
            System.Windows.Shapes.Polyline line = new System.Windows.Shapes.Polyline();
            line.StrokeThickness = 2;
            line.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(Red)); 
            line.Uid = shUid;
            line.Points = drawingPoints;

            Assert.AreEqual(line.StrokeThickness, 2);
            Assert.AreEqual(((SolidColorBrush)line.Stroke).Color, ((SolidColorBrush)(new BrushConverter().ConvertFrom(Red))).Color);

            Assert.AreEqual(line.Points, drawingPoints);

            //call the delete polyline function 
            _viewModel.freeHand.DeletePolyline(_globCanvas, _WBOps, line);

            //check if line is removed from canvas 
            Assert.AreEqual(_globCanvas.Children.Count, 0);

            _globCanvas.Children.Clear();
        }
    }
}
