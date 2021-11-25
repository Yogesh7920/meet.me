/// <author>Sairoop Bodepudi</author>
/// <created>19/10/2021</created>
/// <summary>
///		This files is the implementation of the
///		Summarizer factory that is used to create
///		and return a fixed summarizer instance.
/// </summary>

namespace Dashboard.Server.Summary
{
	/// <summary>
	/// Factory for the summarizer class
	/// </summary>
	public static class SummarizerFactory
	{
		/// <summary>
		/// Constructor for the factory that generates the summarizer
		/// </summary>
		static SummarizerFactory()
		{
			_summarizer = new Summarizer();
		}

		/// <summary>
		/// Gets the instance of the commonly initialized summarizer
		/// </summary>
		/// <returns> 
		/// Summarizer instance of type ISummarizer
		/// </returns>
		public static ISummarizer GetSummarizer()
		{
			return _summarizer;
		}

		/// <summary>
		/// The single instance of the Summarizer which 
		/// would store all the initialized variables 
		/// required for the summarizer
		/// </summary>
		private static readonly ISummarizer _summarizer;
	}
}
