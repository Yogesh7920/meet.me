/// <author>Yogesh</author>
/// <created>26/10/2021</created>


using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Client;
using Networking;
using NUnit.Framework;
using Whiteboard;

namespace Testing.E2E.Yogesh
{
    [TestFixture , Apartment(System.Threading.ApartmentState.STA)]
    public class Whiteboard
    {
        private ISerializer _serializer;
        private Canvas _globCanvas;
        private WhiteBoardViewModel _viewModel;
        private IWhiteBoardOperationHandler _WBOps;

        [OneTimeSetUp]
        public void Setup()
        {
            // Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            // _serializer = new Serializer();
            // _globCanvas = new Canvas() { Height = 300, Width = 500 };
            // _viewModel = new WhiteBoardViewModel(_globCanvas);
            // _WBOps = new WhiteBoardOperationHandler(new Coordinate(((int)_globCanvas.Height), ((int)_globCanvas.Width)));
        }

        [Test]
        public void CreateShape()
        {
            // Random r = new Random();
            // int twidth = 30;
            // int theight = 10;
            //
            // //Mouse points 
            // Point strt;
            // Point end;
            //
            // strt.X = r.Next(1, (int)_globCanvas.Width - (twidth + 1));
            // strt.Y = r.Next(1, (int)_globCanvas.Height - (theight + 1));
            //
            // //set mouse down i.e. starting point  
            // end = strt;
            // //mouse move s
            // for (int i = 1; i <= twidth; i++)
            // {
            //     for (int j = 1; j <= theight; j++)
            //     {
            //         end.X = strt.X + i;
            //         end.Y = strt.Y + j;
            //     }
            // }
            //
            // _globCanvas = _viewModel.shapeManager.CreateShape(_globCanvas, _WBOps, 
            //     WhiteBoardViewModel.WBTools.NewRectangle, strt, end, shapeComp: true);
        }
    }
}