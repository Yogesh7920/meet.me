/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 11/28/2021
**/

using System;

namespace Whiteboard
{
    /// <summary>
    ///     Class used for storing objects in state Manager.
    /// </summary>
    public class BoardShape
    {
        /// <summary>
        ///     Constructor for creating BoardShape.
        /// </summary>
        /// <param name="mainShapeDefiner">The MainShape Object with shape Details.</param>
        /// <param name="userLevel">User Level of creator of shape.</param>
        /// <param name="creationTime">Timestamp of creation of shape.</param>
        /// <param name="lastModifiedTime">Timestamp of last modification operation performed on state.</param>
        /// <param name="uid">Uid of shape.</param>
        /// <param name="shapeOwnerId">Id of owner of shape.</param>
        /// <param name="operation">Recent operation on shape.</param>
        public BoardShape(MainShape mainShapeDefiner, int userLevel, DateTime creationTime, DateTime lastModifiedTime,
            string uid, string shapeOwnerId, Operation operation)
        {
            MainShapeDefiner = mainShapeDefiner;
            UserLevel = userLevel;
            CreationTime = creationTime;
            LastModifiedTime = lastModifiedTime;
            Uid = uid;
            ShapeOwnerId = shapeOwnerId;
            RecentOperation = operation;
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public BoardShape()
        {
            MainShapeDefiner = null;
            UserLevel = 0;
            CreationTime = DateTime.Now;
            LastModifiedTime = DateTime.Now;
            Uid = null;
            ShapeOwnerId = null;
            RecentOperation = Operation.None;
        }

        /// <summary>
        ///     The MainShape Object with shape Details.
        /// </summary>
        public MainShape MainShapeDefiner { get; set; }

        /// <summary>
        ///     User Level of creator of shape.
        /// </summary>
        public int UserLevel { get; set; }

        /// <summary>
        ///     Time of creation of shape.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        ///     Time of last Modification of shape.
        /// </summary>
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        ///     Id of the shape.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        ///     User Id of shape Owner.
        /// </summary>
        public string ShapeOwnerId { get; set; }

        /// <summary>
        ///     Recent operation on shape.
        /// </summary>
        public Operation RecentOperation { get; set; }

        /// <summary>
        ///     Clone for DateTime Object.
        /// </summary>
        /// <param name="dt">DataTime object to be cloned.</param>
        /// <returns>Cloned DateTime object.</returns>
        private static DateTime DataTimeClone(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        /// <summary>
        ///     Creates Clone BoardShape Object.
        /// </summary>
        /// <returns>Cloned Boardshape.</returns>
        public BoardShape Clone()
        {
            return new(MainShapeDefiner.Clone(), UserLevel, DataTimeClone(CreationTime),
                DataTimeClone(LastModifiedTime), (string) Uid.Clone(),
                (string) ShapeOwnerId.Clone(), RecentOperation);
        }
    }
}