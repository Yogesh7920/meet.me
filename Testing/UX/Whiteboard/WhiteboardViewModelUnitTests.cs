using Client;
using NUnit.Framework;
using Whiteboard;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System;

namespace Testing.UX.Whiteboard
{
    [TestFixture , Apartment(System.Threading.ApartmentState.STA)]
    public class WhiteboardViewModelUnitTests
    {
        private WhiteBoardViewModel _viewModel;
        private Canvas _globCanvas;
        private IWhiteBoardOperationHandler _WBOps;


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
            _globCanvas.Children.Add(sh);

            _globCanvas = _viewModel.shapeManager.SelectShape(_globCanvas, sh, _WBOps);


            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);

            _globCanvas = _viewModel.shapeManager.UnselectAllBB(_globCanvas, _WBOps);
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 0);

        }

        [Test]
        public void testMove1()
        {
            //Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            string uid = "auniqueidentifier";
            Shape sh = new System.Windows.Shapes.Rectangle() { Height = 10, Width = 30, Uid = uid };
            
            Random r = new Random();
            int left = r.Next(1, (int)_globCanvas.Width - ((int)sh.Width + 1));
            int top = r.Next(1, (int)_globCanvas.Height - ((int)sh.Height + 1));

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
            for (int i = 1; i <= 20; i++)
            {
                end.X = strt.X + i;
                end.Y = strt.Y + i; 
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
        }

        //[Test]
        /*public void testCreate1()
        {
            //Without calling DispatcherUtil.DoEvents() the test will fail
            //DispatcherUtil.DoEvents();

            Random r = new Random();
            int twidth = 30;  
            int theight = 10;

            //Mouse points 
            Point strt;
            Point temp;
            Point end;

            strt.X = r.Next(1, (int)_globCanvas.Width - (twidth + 1));
            strt.Y = r.Next(1, (int)_globCanvas.Height - (theight + 1));

            //set mouse down i.e. starting point  
            temp = strt;
            end = strt;
            //mouse move s
            for (int i = 1; i <= 20; i++)
            {
                end.X = strt.X + i;
                end.Y = strt.Y + i;
                _globCanvas = _viewModel.shapeManager.CreateShape(_globCanvas, _WBOps,WhiteBoardViewModel.WBTools.NewRectangle ,temp, end, shapeComp: false);
                temp = end;
            }

            //mouse up 
            _globCanvas = _viewModel.shapeManager.CreateShape(_globCanvas, _WBOps, WhiteBoardViewModel.WBTools.NewRectangle, temp, end, shapeComp: true);

            double dX = end.X - strt.X;
            double dY = end.Y - strt.Y;

            int final_left = (int)Canvas.GetLeft(sh);
            int final_top = (int)Canvas.GetTop(sh);

            int final_width = (int)sh.Width; 

            

            //Shape should be still selected 
            Assert.IsTrue(_viewModel.shapeManager.selectedShapes.Contains(uid));
            Assert.AreEqual(_viewModel.shapeManager.selectedShapes.Count, 1);
        }*/
    }
}
