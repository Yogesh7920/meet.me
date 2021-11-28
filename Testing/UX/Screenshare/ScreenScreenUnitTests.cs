using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.ViewModel;
using NUnit.Framework;

namespace Testing.UX.Screenshare
{
    [TestFixture]
    class ScreenScreenUnitTests
    {
        public ScreenShareViewModel Model;

        [SetUp]
        public void SetUp()
        {
            Model = new ScreenShareViewModel(true);
        }

        /// <summary>
        ///     Checking whether event is raised on OnPropertyChanged
        /// </summary>
        [Test]
        public void OnPropertyChanged_Event()
        {
            //Arrange
            string checkProperty = "";
            Model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                checkProperty = e.PropertyName;
            };

            //Act
            Model.OnPropertyChanged("testing");

            //Assert
            Assert.IsNotNull(checkProperty);
            Assert.AreEqual("testing", checkProperty);
        }

    }
}
