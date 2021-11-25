/// <author>Parmanand Kumar</author>
/// <created>15/11/2021</created>
/// <summary>
///     It contains the ResponseEntity Class, which is required as the return type
///     of save functions implemeted 
/// </summary>


namespace Dashboard.Server.Persistence
{
    ///<summary>
    /// It is the return type for save functions, as for this module
    /// It saves the data with variable/different name of files, hence it should be returned.
    ///</summary>
    public class ResponseEntity
    {
        // True if file saved successfully
        public bool IsSaved;

        // name of the file 
        public string FileName;
    }

}
