/// <author>Sairoop Bodepudi</author>
/// <created>3/11/2021</created>
/// <summary>
///		This file contains the processor which
///		processes the string chat and returns
///		the summary of the chat discussion.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dashboard.Server.Summary
{
	/// <summary>
	/// Exception for empty string input for summarizer
	/// </summary>
	public class EmptyStringException : Exception
	{
		public EmptyStringException() { }
	}

	/// <summary>
	/// The main chat processor that completes the preprocessing as well
	/// summarization of a given string.
	/// </summary>
	class ChatProcessor
	{
		/// <summary>
		/// Constructor which initializes the stemmer, the random sampler and 
		/// constructs the stopwords array.
		/// </summary>
		public ChatProcessor()
		{
			_stemmer = new PorterStemmer();
			_random = new Random();
			_stopwords = new string[] { "a", "about", "above", "after", "again",
				"against", "ain", "all", "am", "an", "and", "any", "are", "aren",
				"aren't", "as", "at", "be", "because", "been", "before", "being",
				"below", "between", "both", "but", "by", "can", "couldn", "couldn't",
				"d", "did", "didn", "didn't", "do", "does", "doesn", "doesn't",
				"doing", "don", "don't", "down", "during", "each", "few", "for",
				"from", "further", "had", "hadn", "hadn't", "has", "hasn", "hasn't",
				"have", "haven", "haven't", "having", "he", "her", "here", "hers",
				"herself", "him", "himself", "his", "how", "i", "if", "in", "into",
				"is", "isn", "isn't", "it", "it's", "its", "itself", "just", "ll",
				"m", "ma", "me", "mightn", "mightn't", "more", "most", "mustn",
				"mustn't", "my", "myself", "needn", "needn't", "no", "nor", "not",
				"now", "o", "of", "off", "on", "once", "only", "or", "other", "our",
				"ours", "ourselves", "out", "over", "own", "re", "s", "same",
				"shan", "shan't", "she", "she's", "should", "should've", "shouldn",
				"shouldn't", "so", "some", "such", "t", "than", "that", "that'll",
				"the", "their", "theirs", "them", "themselves", "then", "there",
				"these", "they", "this", "those", "through", "to", "too", "under",
				"until", "up", "ve", "very", "was", "wasn", "wasn't", "we", "were",
				"weren", "weren't", "what", "when", "where", "which", "while", "who",
				"whom", "why", "will", "with", "won", "won't", "wouldn", "wouldn't",
				"y", "you", "you'd", "you'll", "you're", "you've", "your", "yours",
				"yourself", "yourselves", "could", "he'd", "he'll", "he's", "here's",
				"how's", "i'd", "i'll", "i'm", "i've", "let's", "ought", "she'd",
				"she'll", "that's", "there's", "they'd", "they'll", "they're",
				"they've", "we'd", "we'll", "we're", "we've", "what's", "when's",
				"where's", "who's", "why's", "would" };
		}

		/// <summary>
		/// Function to initially tokenize based on a given regex and then
		/// stem to get the lemmas using the previously defined Porter stemmer
		/// after the removal of the stopwords.
		/// </summary>
		/// <param name="message">
		/// The chat message that is to be tokenized and lemmatized
		/// </param>
		/// <returns>
		/// List of string consisting of the lemmas of each of the 
		/// word in the sentence after processing
		/// </returns>
		private List<string> TokenizeLemmatize(string message)
		{
			// The regex being used is "W+" and stopwords are removed
			// and finally lemmas are obtained
			List<string> tokens = Regex.Split(message.Trim(), @"\W+").Where(s => s != string.Empty).ToList();
			tokens = tokens.Select(t => t.ToLower()).ToList();
			tokens.RemoveAll(_stopwords.Contains);
			return tokens.Select(t => _stemmer.StemWord(t)).ToList();
		}

		/// <summary>
		/// Function to break a discussion chat to get the individual 
		/// messages in a particular chat message and tokenize and lemmatize
		/// that particular message to get the semantic meaning with removal
		/// of the stopwords in the intermediate step.
		/// </summary>
		/// <param name="chat">
		/// The chat which is to be processed by the 
		/// TokenizeLemmatize function 
		/// </param>
		/// <returns>
		/// List of string sentences of chats and 
		/// their corresponding semantic meanings
		/// along with a dynamic priority
		/// </returns>
		private (List<(string, bool, List<string>)>, float) Preprocess(List<(string, bool)> chat)
		{
			List<(string, bool)> individualMsgs = new();
			float priority = 0;
			foreach (var c in chat)
			{
				priority += c.Item2 ? 1 : 0;
				foreach (string s in c.Item1.Split('.'))
					individualMsgs.Add((s, c.Item2));
			}
			priority = (priority > 0) ? 10 * chat.Count / priority : 1;
			List<(string, bool, List<string>)> semantics = new();
			foreach (var t in individualMsgs)
				semantics.Add((t.Item1.Trim(), t.Item2, TokenizeLemmatize(t.Item1)));
			Trace.WriteLine("Obtained the semantics of the chat.");
			return (semantics, priority);
		}

		/// <summary>
		/// Get the list of all the semantic words which can be used to build a dictionary.
		/// </summary>
		/// <param name="semantics">
		/// The semantics that is obtained by using the preprocessing function
		/// </param>
		/// <returns>
		/// List of all the words used in the chat of discussion
		/// </returns>
		private static List<string> GetWords(List<(string, bool, List<string>)> semantics)
		{
			List<string> words = new();
			foreach (var t in semantics)
				words.AddRange(t.Item3);
			return words;
		}

		/// <summary>
		/// Function to get the vocabulary of the discussion alogn with
		/// the number of occurences of each word for generating
		/// sentence scores.
		/// </summary>
		/// <param name="words">
		/// The list of all the words including 
		/// repeating words in the chat discussion
		/// </param>
		/// <returns>
		/// A dictionary constaining all the unique words 
		/// and their number of occurences
		/// </returns>
		private static Dictionary<string, int> GetVocab(List<string> words)
		{
			Dictionary<string, int> vocab = new();
			foreach (string word in words)
			{
				// Add new unseen words to dictionary using exceptions
				try { vocab.Add(word, 1); }
				catch (ArgumentException) { vocab[word] += 1; }
			}
			return vocab;
		}

		/// <summary>
		/// Get the probability scores for each of the input 
		/// chat sentences based on the semantic meaning.
		/// </summary>
		/// <param name="input">
		/// Input of the preprocessed chat
		/// </param>
		/// <param name="priority">
		/// Priority of starred messages
		/// </param>
		/// <returns>
		/// List of sentence strings and their corresponding scores
		/// </returns>
		private static List<(string, float)> SentenceScores(List<(string, bool, List<string>)> input, float priority)
		{
			Dictionary<string, int> vocab = GetVocab(GetWords(input));
			List<(string, float)> sentenceScore = new();
			float allSentenceScore = 0;
			foreach (var item in input)
			{
				float score = 0;
				foreach (string lemma in item.Item3)
					// Raw score is the frequency sum of all the words
					// in the sentence's semantic meaning
					score += item.Item2 ? priority * vocab[lemma] : vocab[lemma];
				sentenceScore.Add((item.Item1, score));
				allSentenceScore += score;
			}
			// Probability score would be raw score divided
			// by total number of words in the chat
			Trace.WriteLine("Processed the sentences to obtain raw scores.");
			return sentenceScore.Select(t => (t.Item1, t.Item2 / allSentenceScore)).ToList();
		}

		/// <summary>
		/// Sample sentences based on probabilty score 
		/// obtained using SentenceScores function. 
		/// </summary>
		/// <param name="input">
		/// Input string which is concatenation of all the chat strings
		/// </param>
		/// <param name="fraction">
		/// Fraction of the summary string in comparision to original chat
		/// </param>
		/// <returns>
		/// List of strings which are sampled using the 
		/// given probability score distribution
		/// </returns>
		/// <remarks>
		/// Reference : https://stackoverflow.com/a/43345968
		/// </remarks>
		private List<string> SummarySamples(List<(string, bool)> input, float fraction)
		{
			float sum = 0;
			List<string> summary = new();
			(List<(string, bool, List<string>)> processedChat, float priority) = Preprocess(input);
			List<(string, float)> scores = SentenceScores(processedChat, priority);
			// Get the cummulative distributive scores for the sampling process
			List<(string, float)> cdfScores = scores.Select(s =>
			{
				var res = s.Item2 + sum;
				sum += s.Item2;
				return (s.Item1, res);
			}).ToList();
			bool isEmpty = true;
			foreach(var s in cdfScores)
			{
				if (s.Item1 != "")
				{
					isEmpty = false;
					break;
				}
			}
			if (isEmpty)
				throw new EmptyStringException();
			int size = Convert.ToInt32(fraction * cdfScores.Count);
			while (summary.Count < size)
			{
				float p = (float)_random.NextDouble();
				foreach (var item in cdfScores.ToList())
				{
					// The first string sentence with probability score
					// greater than the given random value is sampled
					// and is removed to avoid repeating sentences
					if (item.Item2 >= p)
					{
						summary.Add(item.Item1);
						cdfScores.RemoveAll(s => s == item);
						break;
					}
				}
			}
			Trace.WriteLine("Succesfully sampled summary sentences of given length.");
			return summary;
		}

		/// <summary>
		/// Function that encompasses the SummarySamples and generates 
		/// the summary string in presence of non-empty chat.
		/// </summary>
		/// <param name="chat">
		/// Input chat in the discussion after the concatenation
		/// </param>
		/// <returns>
		/// A summary string which presents the summary as 
		/// all important points of the chats in the discussion
		/// </returns>
		public string Summarize(List<(string, bool)> chat)
		{
			int length = chat.Count;
			float fraction;
			if (length > 20)
				fraction = (float)10 / length;
			else
				fraction = (float)(20 - length) / 20;
			try
			{
				List<string> summary = SummarySamples(chat, fraction);
				return string.Join(". ", summary);
			}
			catch (EmptyStringException)
			{
				Trace.WriteLine("Empty String encountered for summarizing.");
				return "";
			}
		}

		/// <summary>
		/// Stemming algorithm that uses the Porter Stemming algorithm.
		/// </summary>
		private readonly PorterStemmer _stemmer;

		/// <summary>
		/// Sampler helper for randomly sampling the summary based
		/// on a given probablity distribution.
		/// </summary>
		private readonly Random _random;

		/// <summary>
		/// An array of stopwords string in english which is derived
		/// from : https://gist.github.com/sebleier/554280
		/// </summary>
		private readonly string[] _stopwords;
	}
}