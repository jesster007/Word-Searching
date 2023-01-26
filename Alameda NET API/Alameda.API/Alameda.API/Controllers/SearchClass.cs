using Alameda.API.Model;

namespace Alameda.API.Controllers
{
    public class SearchClass
    {
        public string[] searchString(string input, List<Tuple<string, TokenType>> queryTokens, Boolean matchWhole, Boolean matchCase)
        {
            string searchInput = input;
            if (matchCase == false)
                searchInput = searchInput.ToLower();

            Stack<string> tokenStack = new Stack<string>();
            List<string> foundL = new List<string>();
            int i = 0, qInd = 0;
            while (i < searchInput.Length && qInd < queryTokens.Count)
            {
                if (queryTokens[qInd].Item2 == TokenType.Operator) //Handle preceding tokens
                {
                    tokenStack.Push(queryTokens[qInd].Item1);
                    qInd++;
                }
                else if (queryTokens[qInd].Item2 == TokenType.String)
                {
                    int sStart = 0; //Start of the string to return
                    int resultStart = searchInput.IndexOf(queryTokens[qInd].Item1, i);
                    if (resultStart != -1)
                    {
                        int resultEnd = resultStart + queryTokens[qInd].Item1.Length;
                        bool prevValid = false, nextValid = false;

                        int prevSpace = searchInput.LastIndexOf(" ", resultStart, resultStart); //Get the index of the last space before the string

                        if (tokenStack.Count > 0) //If there are tokens to validate
                        {
                            Tuple<int, int> minMaxChar = calcTokens(ref tokenStack, matchWhole);
                            if (prevSpace != -1 && minMaxChar.Item2 >= resultStart - (prevSpace + 1) && resultStart - (prevSpace + 1) >= minMaxChar.Item1) //If the previous space is in range
                            {
                                prevValid = true;
                                sStart = prevSpace + 1;
                            }
                            else if (prevSpace == -1 && resultStart >= minMaxChar.Item1 && resultStart <= minMaxChar.Item2) //If there is no previous space, proceed if the start is in range 
                                prevValid = true;
                        }
                        else if (!matchWhole) //If there are no tokens, just accept left side
                        {
                            prevValid = true;
                            if (prevSpace != -1)
                                sStart = prevSpace + 1;
                            else
                                sStart = 0;
                        }
                        else if (prevSpace != -1 && resultStart - prevSpace == 1) //If the previous space is immediately preceding the result
                        {
                            sStart = resultStart;
                            prevValid = true;
                        }
                        else if (resultStart == 0) //If there is nothing to the left
                            prevValid = true;
                        qInd++;

                        int nextMinGap = 0, nextMaxGap = 0; //Keep track of the range for the next gap
                        while (qInd <= queryTokens.Count)
                        {
                            int nextSpace = searchInput.IndexOf(" ", resultEnd); //Get the index of the next space after the string
                            if (qInd == queryTokens.Count) //If there is no more to the query --base case
                            {
                                if (!matchWhole) //Any extra characters on the sides are allowed
                                    nextMaxGap = int.MaxValue;
                                if (nextSpace == -1 && searchInput.Length - resultEnd >= nextMinGap && searchInput.Length - resultEnd <= nextMaxGap) //If there are no more spaces, check if the end is close enough
                                {
                                    nextValid = true;
                                    resultEnd = searchInput.Length;
                                }
                                else if (prevValid && nextSpace != -1 && nextSpace - resultEnd <= nextMaxGap && nextSpace - resultEnd >= nextMinGap) //If the next space is an appropriate distance away
                                {
                                    nextValid = true;
                                    resultEnd = nextSpace;
                                }
                            }
                            else if (queryTokens[qInd].Item2 == TokenType.Operator) //Handle the operators
                            {
                                switch (queryTokens[qInd].Item1)
                                {
                                    case "?":
                                        if (nextMaxGap != int.MaxValue)
                                            nextMaxGap++;
                                        nextMinGap++;
                                        break;
                                    case "*":
                                        nextMaxGap = int.MaxValue;
                                        break;
                                }
                            }
                            else if (queryTokens[qInd].Item2 == TokenType.String)  //Search for the additional strings
                            {
                                int sIndex = searchInput.IndexOf(queryTokens[qInd].Item1, resultEnd);
                                if (sIndex == -1)
                                    break;
                                if (sIndex - resultEnd >= nextMinGap && sIndex - resultEnd <= nextMaxGap) //If the next string is an appropriate spacing away
                                {
                                    resultEnd = sIndex + queryTokens[qInd].Item1.Length; //Move the end of the total string
                                    nextMinGap = 0; nextMaxGap = 0;
                                }
                                else
                                    break;

                            }
                            qInd++;
                        }
                        if (prevValid && nextValid && input.Substring(sStart, resultEnd - sStart) != " ")
                            foundL.Add(input.Substring(sStart, resultEnd - sStart));

                        qInd = 0;
                        i = resultEnd + 1;
                    }
                    else //If the string was not found
                    {
                        tokenStack = new Stack<string>();
                        break;
                    }
                }
            }
            if (tokenStack.Count > 0) //If there were tokens at the start and no following ones
            {
                Tuple<int, int> minMaxChar = calcTokens(ref tokenStack, matchWhole);
                foreach (string word in input.Split(" "))
                    if (word.Length >= minMaxChar.Item1 && word.Length <= minMaxChar.Item2 && word != "")
                        foundL.Add(word);

            }
            string[] found = foundL.ToArray();
            return (found);
        }

        public Tuple<int, int> calcTokens(ref Stack<string> tokens, Boolean matchWhole)
        {
            int maxAllowed = 0;
            int minAllowed = 0;
            while (tokens.Count > 0)
            {
                string curToken = tokens.Pop();
                if (curToken == "*")
                    maxAllowed = int.MaxValue; //Any number of letters
                else if (curToken == "?")
                {
                    if (maxAllowed != int.MaxValue)
                        maxAllowed++;
                    minAllowed++;
                }
            }
            if (!matchWhole)
                maxAllowed = int.MaxValue;
            return Tuple.Create(minAllowed, maxAllowed);
        }
        public List<Tuple<string, TokenType>> parseQuery(string query)
        {
            //Later Idea = use split notation?
            List<Tuple<string, TokenType>> tokenList = new List<Tuple<string, TokenType>>();
            string currentToken = "";
            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '*' || query[i] == '?')
                {
                    if (currentToken.Length > 0)
                        tokenList.Add(Tuple.Create(currentToken, TokenType.String));
                    currentToken = "";
                    tokenList.Add(Tuple.Create(Convert.ToString(query[i]), TokenType.Operator));
                }
                else if (query[i] == '~')
                {
                    if (i < query.Length - 1 && (query[i + 1] == '*' || query[i + 1] == '?' || query[i + 1] == '~'))
                    {
                        currentToken += query[i + 1];
                        i++;
                    }
                }
                else
                    currentToken += query[i];
            }
            if (currentToken.Length > 0) //Handle ending strings
                tokenList.Add(Tuple.Create(currentToken, TokenType.String));
            return tokenList;
        }

        public Tuple<string, string> removePunctuation(string input, string query)
        {
            string newInput = "";
            string newQuery = "";

            foreach (char c in input)
                if (!char.IsPunctuation(c)) //Remove punctuation from the input
                    newInput += c;
            foreach (char c in query)
                if (!char.IsPunctuation(c) || (c == '*' || c == '?' || c == '~')) //Remove any punctuation but operators from the query
                    newQuery += c;

            input = newInput.ToString();
            query = newQuery.ToString();
            return Tuple.Create(input, query);
        }
    }
}
