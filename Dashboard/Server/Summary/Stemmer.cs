namespace Dashboard.Server.Summary
{
    /// <summary>
    /// The Stemmer class transforms a word into its root form.
    /// Implementing the Porter Stemming Algorithm
    /// </summary>
    /// <remarks>
    /// Reference : http://snowball.tartarus.org/algorithms/porter/stemmer.html
    /// </remarks>
    public class PorterStemmer
    {
        /// <summary>
        /// Returns whether the character at a given 
        /// index is a consonant or not.
        /// </summary>
        /// <param name="index">
        /// Index of the character in the string to 
        /// be checked
        /// </param>
        /// <returns>
        /// True if character is consonant and 
        /// false otherwise
        /// </returns>
        /// <remarks>
        /// A consonant in a word is a letter other than 
        /// A, E, I, O or U, and other than Y preceded 
        /// by a consonant.
        /// </remarks>
        private bool IsConsonant(int index)
        {
            var c = wordArray[index];
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') return false;
            return c != 'y' || (index == 0 || !IsConsonant(index - 1));
        }

        /// <summary>
        /// Moves forward as long as the successive element is of the
        /// same type (vowel or consonant) as the current element.
        /// </summary>
        /// <param name="consonant">
        /// If the character to be checked is consonant i.e move as long 
        /// as we see a consonant and vice versa for the vowel
        /// </param>
        /// <returns>
        /// True if the end of the stem is reached and false otherwise
        /// </returns>
        private bool MoveForward(bool consonant)
        {
            while (true)
            {
                if (index > stemIndex)
                    return false;
                if (IsConsonant(index) == consonant)
                    break;
                index++;
            }
            return true;
        }

        /// <summary>
        /// Returns the measure of a stem.
        /// </summary>
        /// <returns>
        /// Measure of the stem
        /// </returns>
        /// <remarks>
        /// Any word can be represented in the form [C](VC)^m[V] where
        /// a list ccc... of length greater than 0 will be denoted by C,
        /// and a list vvv... of length greater than 0 will be denoted by V.
        /// Here m will be called the measure of any word or word part when 
        /// represented in this form
        /// </remarks>
        private int GetStemMeasure()
        {
            n = 0;
            index = 0;
            if (!MoveForward(false))
                return n;
            index++;
            while (true)
            {
                if (!MoveForward(true))
                    return n;
                index++;
                n++;
                if (!MoveForward(false))
                    return n;
                index++;
            }
        }


        /// <summary>
        /// Function that checks whether the stem of the string 
        /// in current state preceeds a given string s.
        /// </summary>
        /// <param name="s">
        /// String that is to be checked if present after the stem.
        /// </param>
        /// <returns>
        /// True if the given string succeeds the input string 
        /// stem and false otherwise.
        /// </returns>
        private bool StemEndsWith(string s)
        {
            var length = s.Length;
            var index = endIndex - length + 1;
            if (index < 0) return false;

            string word = new string(wordArray);
            bool retVal = word.Substring(index, length) == s;
            // If it does then update the stem index based on the
            // length of the string that is checked and original stem
            stemIndex = retVal ? endIndex - length : stemIndex;
            return retVal;
        }

        /// <summary>
        /// Check if the stem contains a vowel.
        /// </summary>
        /// <returns>
        /// True if stem contains vowel and false otherwise
        /// </returns>
        private bool StemContainsVowel()
        {
            for (int i = 0; i <= stemIndex; i++)
                if (!IsConsonant(i)) return true;
            return false;
        }

        /// <summary>
        /// Checks if the stem in the input stem ends 
        /// with a double consonant.
        /// </summary>
        /// <param name="index">
        /// Endindex of the stem
        /// </param>
        /// <returns>
        /// True if the rule *d satisfies and false otherwise
        /// </returns>
        private bool StemEndsDoubleConsonant(int index)
        {
            if (index < 1) return false;
            return wordArray[index] == wordArray[index - 1] && IsConsonant(index);
        }

        /// <summary>
        /// Checks for whether the stem ends cvc, where the second c 
        /// is not W, X or Y.
        /// </summary>
        /// <param name="index">
        /// Endindex of the stem
        /// </param>
        /// <returns>
        /// True if the rule *o satisfies and false otherwise
        /// </returns>
        private bool EndsCVC(int index)
        {
            if (index < 2 || !IsConsonant(index) || IsConsonant(index - 1) || !IsConsonant(index - 2)) return false;
            char c = wordArray[index];
            return c != 'w' && c != 'x' && c != 'y';
        }

        /// <summary>
        /// Set the end of the string after the stem 
        /// with the given input string.
        /// </summary>
        /// <param name="s">
        /// String that is used to set the end of the stem
        /// </param>
        private void SetEnd(string s)
        {
            var length = s.Length;
            var index = stemIndex + 1;
            for (int i = 0; i < length; i++)
                wordArray[index + i] = s[i];
            endIndex = stemIndex + length;
        }

        /// <summary>
        /// Set the end only if the measure of the 
        /// stem is greater than 0.
        /// </summary>
        /// <param name="s">
        /// The string that is used to set the end 
        /// of the stem
        /// </param>
        private void ConditionalSetEnd(string s)
        {
            if (GetStemMeasure() > 0)
                SetEnd(s);
        }

        /// <summary>
        /// Follows the Step 1a as in the reference.	
        /// </summary>
        private void Step1a()
        {
            if (StemEndsWith("sses"))
                endIndex -= 2;
            else if (StemEndsWith("ies"))
                SetEnd("i");
            else if (wordArray[endIndex - 1] != 's' && wordArray[endIndex] == 's')
                endIndex--;
        }

        /// <summary>
        /// Check if the given character is not an l, s, z.
        /// </summary>
        /// <param name="c">
        /// The character to be checked
        /// </param>
        /// <returns>
        /// True if the character is not l, s, z and false otherwise
        /// </returns>
        private static bool CharNotLSZ(char c)
        {
            return !(c == 'l' || c == 's' || c == 'z');
        }

        /// <summary>
        /// If the second or third of the rules in Step 1b of 
        /// reference is successful then this check is done.
        /// </summary>
        /// <remarks>
        /// The rule to map to a single letter causes the removal 
        /// of one of the double letter pair. The -E is put back
        /// on -AT, -BL and -IZ, so that the suffixes -ATE, -BLE 
        /// and -IZE can be recognised later. This E may be removed 
        /// in step 4.
        /// </remarks>
        private void Step1bCheck()
        {
            char c = wordArray[endIndex];
            if (StemEndsWith("at"))
                SetEnd("ate");
            else if (StemEndsWith("bl"))
                SetEnd("ble");
            else if (StemEndsWith("iz"))
                SetEnd("ize");
            else if (StemEndsDoubleConsonant(endIndex) && CharNotLSZ(c))
                endIndex--;
            else if (GetStemMeasure() == 1 && EndsCVC(endIndex))
                SetEnd("e");
        }

        /// <summary>
        /// Follows the Step 1b as in the reference.	
        /// </summary>
        private void Step1b()
        {
            if (StemEndsWith("eed") && GetStemMeasure() > 0)
                endIndex--;
            else if ((StemEndsWith("ed") || StemEndsWith("ing")) && StemContainsVowel())
            {
                endIndex = stemIndex;
                Step1bCheck();
            }
        }

        /// <summary>
        /// Follows the Step 1c as in the reference.	
        /// </summary>
        private void Step1c()
        {
            if (StemEndsWith("y") && StemContainsVowel())
                wordArray[endIndex] = 'i';
        }

        /// <summary>
        /// Step 1 deals with plurals and past participles.
        /// </summary>
        private void Step1()
        {
            Step1a();
            Step1b();
            Step1c();
        }

        /// <summary>
        /// Does a conditional end change based on the step if and only
        /// if the previous rule has not been matched.
        /// </summary>
        /// <param name="source">
        /// The end string to be changed in the original input
        /// </param>
        /// <param name="replace">
        /// The end string that is replacing the original input end string
        /// </param>
        /// <param name="step">
        /// Determines the step in the stemming process
        /// </param>
        private void EndChange(string source, string replace, int step = 2)
        {
            // Determine if the step has already been
            // completed by a previous rule
            bool check = step == 2 ? step2 : step3;
            if (StemEndsWith(source) && check)
            {
                ConditionalSetEnd(replace);
                if (step == 2)
                    step2 = false;
                else if (step == 3)
                    step3 = false;
            }
        }

        /// <summary>
        /// Implements the Step 2 as present in the reference.
        /// </summary>
        private void Step2()
        {
            if (endIndex == 0)
                return;
            EndChange("ational", "ate");
            EndChange("tional", "tion");
            EndChange("enci", "ence");
            EndChange("anci", "ance");
            EndChange("izer", "ize");
            EndChange("bli", "ble");
            EndChange("alli", "al");
            EndChange("entli", "ent");
            EndChange("eli", "e");
            EndChange("ousli", "ous");
            EndChange("ization", "ize");
            EndChange("ation", "ate");
            EndChange("ator", "ate");
            EndChange("alism", "al");
            EndChange("iveness", "ive");
            EndChange("fulness", "ive");
            EndChange("ousness", "ous");
            EndChange("aliti", "al");
            EndChange("iviti", "ive");
            EndChange("biliti", "ble");
        }

        /// <summary>
        /// Implements the Step 3 as present in the reference.
        /// </summary>
        private void Step3()
        {
            if (endIndex == 0)
                return;
            EndChange("icate", "ic", 3);
            EndChange("ative", "", 3);
            EndChange("alize", "al", 3);
            EndChange("iciti", "ic", 3);
            EndChange("ical", "ic", 3);
            EndChange("ful", "", 3);
            EndChange("ness", "", 3);
        }

        /// <summary>
        /// Find the stem index and check if the stem ends 
        /// with th query string and complete the rule if
        /// already not completed.
        /// </summary>
        /// <param name="source">
        /// The query string to be checked
        /// </param>
        private void EndsWithStep4(string source)
        {
            if (step4)
            {
                if (StemEndsWith(source))
                    step4 = false;
            }
        }

        /// <summary>
        /// This removes all the suffixes and implements 
        /// the Step 4 as present in the reference.
        /// </summary>
        private void Step4()
        {
            if (endIndex == 0)
                return;
            EndsWithStep4("al");
            EndsWithStep4("ance");
            EndsWithStep4("ence");
            EndsWithStep4("er");
            EndsWithStep4("ic");
            EndsWithStep4("able");
            EndsWithStep4("ible");
            EndsWithStep4("ant");
            EndsWithStep4("ement");
            EndsWithStep4("ment");
            EndsWithStep4("ent");
            if (step4)
            {
                if (StemEndsWith("ion") && stemIndex >= 0 && (wordArray[stemIndex] == 's' || wordArray[stemIndex] == 't'))
                    step4 = false;
            }
            EndsWithStep4("ou");
            EndsWithStep4("ism");
            EndsWithStep4("ate");
            EndsWithStep4("iti");
            EndsWithStep4("ous");
            EndsWithStep4("ive");
            EndsWithStep4("ize");
            if (!step4 && GetStemMeasure() > 1)
                endIndex = stemIndex;
        }

        /// <summary>
        /// Implements the Step 5a in the reference
        /// </summary>
        private void Step5a()
        {
            if (wordArray[endIndex] == 'e')
            {
                var a = GetStemMeasure();
                if (a > 1 || (a == 1 && !EndsCVC(endIndex - 1)))
                    endIndex--;
            }
        }

        /// <summary>
        /// Implements the Step 5b in the reference
        /// </summary>
        private void Step5b()
        {
            if (wordArray[endIndex] == 'l' && StemEndsDoubleConsonant(endIndex) && GetStemMeasure() > 1)
                endIndex--;
        }

        /// <summary>
        /// Implements the Step 5 as a whole which 
        /// completes some tidying work that is remaining.
        /// </summary>
        private void Step5()
        {
            stemIndex = endIndex;
            Step5a();
            Step5b();
        }

        /// <summary>
        /// Function that does the stemming based on 
        /// the Porter's stemming algorithm.
        /// </summary>
        /// <param name="word">
        /// The word that needs to be stemmed
        /// </param>
        /// <returns>
        /// The stemmed word
        /// </returns>
        public string StemWord(string word)
        {
            // No stemming if it is a null word or has length lesser than 2
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2)
                return word;

            wordArray = word.ToCharArray();
            step2 = true;
            step3 = true;
            step4 = true;
            stemIndex = 0;
            endIndex = word.Length - 1;

            Step1();
            Step2();
            Step3();
            Step4();
            Step5();

            return new string(wordArray, 0, endIndex + 1);
        }

        /// <summary>
        /// A word array that would be easier to
        /// manipulate rather than a string.
        /// </summary>
        private char[] wordArray;

        /// <summary>
        /// Bool to check if step 2 is completed.
        /// </summary>
        private bool step2;

        /// <summary>
        /// Bool to check if step 3 is completed.
        /// </summary>
        private bool step3;

        /// <summary>
        /// Bool to check if step 4 is completed.
        /// </summary>
        private bool step4;

        /// <summary>
        /// Index which would be required to find the stem measure.
        /// </summary>
        private int index;

        /// <summary>
        /// Denotes the stem measure
        /// </summary>
        private int n;

        /// <summary>
        /// End index of the string which is 
        /// manipulated throughout the stemming.
        /// </summary>
        private int endIndex;

        /// <summary>
        /// Index of the stem which is 
        /// manipulated throughout the stemming.
        /// </summary>
        private int stemIndex;
    }
}