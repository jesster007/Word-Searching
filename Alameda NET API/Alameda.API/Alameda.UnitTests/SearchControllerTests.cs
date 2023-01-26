namespace Alameda.UnitTests
{
    [TestClass]
    public class SearchClassTests
    {
        public string alphabet = "abcdefghijklmnopqrstuvwxyz";
        public string punctuation = ",./\"\'\\#![]{};:";
        public string wildcardOperators = "?*";
        public string tilda = "~";

        [TestMethod]
        public void RemovePunctuation_NoPunctuation_ReturnsSameInput()
        {
            SearchClass search = new SearchClass();

            string testInput = createString(20, alphabet);
            string testQuery = createString(10, alphabet);

            Tuple<string, string> resultText = search.removePunctuation(testInput, testQuery);

            Assert.AreEqual(testInput, resultText.Item1);
            Assert.AreEqual(testQuery, resultText.Item2);
        }
        [TestMethod]
        public void RemovePunctuation_Punctuation_ReturnsStringWithoutPunctuation()
        {
            SearchClass search = new SearchClass();

            string testInput = createString(20, alphabet);
            string testInputWithPunc = testInput + createString(5, punctuation);
            string testQuery = createString(10, alphabet);
            string testQueryWithPunc = testQuery + createString(5, punctuation);

            Tuple<string, string> resultText = search.removePunctuation(testInputWithPunc, testQueryWithPunc);

            Assert.AreEqual(testInput, resultText.Item1);
            Assert.AreEqual(testQuery, resultText.Item2);
        }

        [TestMethod]
        public void RemovePunctuation_Operators_ReturnsSameQuery()
        {
            SearchClass search = new SearchClass();

            string testQuery = createString(10, alphabet) + createString(5, wildcardOperators);

            string dummyInput = "";
            Tuple<string, string> resultText = search.removePunctuation(dummyInput, testQuery);

            Assert.AreEqual(testQuery, resultText.Item2);
        }

        [TestMethod]
        public void ParseQuery_StringOnly_ReturnsOneStringToken()
        {
            SearchClass search = new SearchClass();

            string testQuery = createString(20, alphabet);

            List<Tuple<string, TokenType>> queryTokens = search.parseQuery(testQuery);

            Assert.IsTrue(queryTokens.Count == 1 && queryTokens[0].Item2 == TokenType.String && queryTokens[0].Item1 == testQuery);
        }
        [TestMethod]
        public void ParseQuery_Operators_ReturnsSeperateOperatorTokens()
        {
            SearchClass search = new SearchClass();
            int numOperators = 10;
            string testQuery = createString(numOperators, wildcardOperators);

            List<Tuple<string, TokenType>> queryTokens = search.parseQuery(testQuery);

            bool isOperator = true;

            if (queryTokens.Count != numOperators)
                isOperator = false;
            foreach (Tuple<string, TokenType> token in queryTokens)
                if (token.Item2 != TokenType.Operator)
                    isOperator =  false;
            Assert.IsTrue(isOperator);
        }

        [TestMethod]
        public void ParseQuery_TildaOperator_ReturnsAllStringComponents()
        {
            SearchClass search = new SearchClass();
            
            string testQuery = "";
            Random random = new Random();
            int numCharacters = random.Next(100);

            for (int i = 0; i < numCharacters; i++)
                testQuery += createString(1, tilda) + createString(1,wildcardOperators);

            List<Tuple<string, TokenType>> queryTokens = search.parseQuery(testQuery);

            Assert.IsTrue(queryTokens.Count == 1 && queryTokens[0].Item2 == TokenType.String && queryTokens[0].Item1.Length == numCharacters);
        }

        [TestMethod]
        public void ParseQuery_EmptyQuery_EmptyTokens()
        {
            SearchClass search = new SearchClass();

            string testQuery = "";
            List<Tuple<string, TokenType>> queryTokens = search.parseQuery(testQuery);

            Assert.IsTrue(queryTokens.Count == 0);
        }
        [TestMethod]
        public void ParseQuery_MixedInput_ReturnsMixedTokens()
        {
            SearchClass search = new SearchClass();

            string testQuery = "";
            Random random = new Random();
            int numTokens = random.Next(100) * 2;

            for (int i = 0; i < numTokens / 2; i++)
                testQuery += createString(5, alphabet) + createString(1, wildcardOperators);

            List<Tuple<string, TokenType>> queryTokens = search.parseQuery(testQuery);

            int countString = 0;
            int countOperator = 0;

            foreach (Tuple<string, TokenType> token in queryTokens)
            {
                if (token.Item2 == TokenType.Operator)
                    countOperator++;
                else if (token.Item2 == TokenType.String) 
                    countString++;
            }
            Assert.IsTrue(queryTokens.Count == numTokens && countString == countOperator);
        }

        [TestMethod]
        public void SearchString_EmptyInputs_EmptyStringArray() { 
            SearchClass search = new SearchClass();
            Assert.IsTrue(search.searchString("", new List<Tuple<string, TokenType>>(), false, false).Length == 0);
        }
        
        [TestMethod]
        public void SearchString_StringInput_StringsFound()
        {
            SearchClass search = new SearchClass();
            string input = "";
            List<string> queryList = new List<string>();
            int numStrings = 50;
            for (int i = 0; i < numStrings; i++)
            {
                string randomString = createString(20, alphabet);
                input += randomString + " ";
                queryList.Add(randomString);
            }

            Boolean allFound = true;
            foreach (string value in queryList)
                if(search.searchString(input, search.parseQuery(value), true, true).Length == 0)
                    allFound = false;
            Assert.IsTrue(allFound);
        }

        [TestMethod]
        public void SearchString_MatchCaseTrue_StringsNotFound()
        {
            SearchClass search = new SearchClass();
            string input = "";
            List<string> queryList = new List<string>();
            int numStrings = 50;
            for (int i = 0; i < numStrings; i++)
            {
                string randomString = createString(20, alphabet);
                input += randomString.ToUpper() + " ";
                queryList.Add(randomString);
            }

            Boolean allFound = true;
            foreach (string value in queryList)
                if (search.searchString(input, search.parseQuery(value), true, true).Length != 0)
                    allFound = false;
            Assert.IsTrue(allFound);
        }

        [TestMethod]
        public void SearchString_MatchCaseFalse_StringsFound()
        {
            SearchClass search = new SearchClass();
            string input = "";
            List<string> queryList = new List<string>();
            int numStrings = 50;
            for (int i = 0; i < numStrings; i++)
            {
                string randomString = createString(20, alphabet);
                input += randomString.ToUpper() + " ";
                queryList.Add(randomString);
            }

            Boolean allFound = true;
            foreach (string value in queryList)
                if (search.searchString(input, search.parseQuery(value), true, false).Length == 0)
                    allFound = false;
            Assert.IsTrue(allFound);
        }

        [TestMethod]
        public void SearchString_MatchWholeTrue_StringsNotFound()
        {
            SearchClass search = new SearchClass();
            string input = "";
            List<string> queryList = new List<string>();
            int numStrings = 50;
            for (int i = 0; i < numStrings; i++)
            {
                string randomString = createString(20, alphabet);
                input += randomString.ToUpper() + " ";
                queryList.Add(randomString.Remove(randomString.Length - 1));
            }

            Boolean allFound = true;
            foreach (string value in queryList)
                if (search.searchString(input, search.parseQuery(value), true, true).Length != 0)
                    allFound = false;
            Assert.IsTrue(allFound);
        }

        [TestMethod]
        public void SearchString_MatchWholeFalse_StringsFound()
        {
            SearchClass search = new SearchClass();
            string input = "";
            List<string> queryList = new List<string>();
            int numStrings = 50;
            for (int i = 0; i < numStrings; i++)
            {
                string randomString = createString(20, alphabet);
                input += randomString.ToUpper() + " ";
                queryList.Add(randomString.Remove(randomString.Length - 1));
            }

            Boolean allFound = true;
            foreach (string value in queryList)
                if (search.searchString(input, search.parseQuery(value), false, true).Length != 0)
                    allFound = false;
            Assert.IsTrue(allFound);
        }

        [TestMethod]
        public void SearchString_Operators_StringsCorrectLength()
        {
            SearchClass search = new SearchClass();
            string input = "";
            int numStrings = 50;
            int range = 10;
            Random random = new Random();

            for (int i = 0; i < numStrings; i++)
                input += createString(random.Next(range), alphabet) + " ";

            Boolean allFound = true;
            for (int i = 0; i < range; i++)
            {
                string query = createString(i, wildcardOperators);
                List<Tuple<string, TokenType>> queryTokens = search.parseQuery(query);
                Stack<string> tokens = new Stack<string>();

                foreach (Tuple<string, TokenType> token in queryTokens) //Put every operator onto the stack
                    tokens.Push(token.Item1);

                Tuple<int, int> minMax = search.calcTokens(ref tokens, true);
                string[] resultString = search.searchString(input, queryTokens, true, true);
                
                foreach (string word in resultString)
                    if (word.Length > minMax.Item2 || word.Length < minMax.Item1) //Make sure every operator is the proper size
                    {
                        allFound = false; 
                        break;
                    }

            }
           
            Assert.IsTrue(allFound);
        }
        public string createString(int length, string characters)
        {
            Random random = new Random();
            string output = "";

            for (int i = 0; i < length; i++)
                output += characters[random.Next(characters.Length)];
            return output;
        }

    }
}