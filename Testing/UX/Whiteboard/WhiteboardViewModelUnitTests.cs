using Client;
using NUnit.Framework;
using Whiteboard;
using System.Windows.Controls;
using System.Windows.Shapes;


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

            _globCanvas.Children.Clear();
        }


    }
}
