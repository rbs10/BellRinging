using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
    /// <summary>
    /// A swapping (of bells)
    /// </summary>
    public class Permutation
    {
        // where each row maps to
        int[] _rowMapping = new int[40320];

        int[] mapping;

        static Dictionary<string, Permutation> _permuations = new Dictionary<string, Permutation>();

        bool[] computed = new bool[8];

        public bool IsCross
        {
            get
            {
                bool ret = true;
                for (int i = 0; i < mapping.Length; ++i )
                {
                    if ( mapping[i] == i )
                    {
                        ret = false;
                    }
                }
                return ret;
            }
        }

        public static Permutation GetPermutation(int[] mapping)
        {
            char[] key = new char[mapping.Length];
            for (int i = 0; i < mapping.Length; ++i)
            {
                key[i] = "ABCDEFGHIJKLMNO"[mapping[i]];
            }
            string sKey = new string(key);
            Permutation perm;
            lock (_permuations)
            {
                if (!_permuations.TryGetValue(sKey, out perm))
                {
                    perm = new Permutation(mapping);
                    _permuations[sKey] = perm;
                }
            }
            return perm;
        }

        private Permutation(int[] mapping)
        {
            this.mapping = mapping;

            for (int i = 0; i < _rowMapping.Length; ++i)
            {
                _rowMapping[i] = -1;
            }
        }


        /// <summary>
        /// Create a permutation from a single definition in the form of a sequence of numbers or 'x'
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="noBells"></param>
        public static Permutation FromPlaceNotation(string definition, int noBells)
        {
            int[] mapping = new int[noBells];
            for (int i = 0; i < noBells; ++i)
            {
                mapping[i] = i;
            }
            int maybePlace = 0;
            int defCursor = 0;
            while (maybePlace < noBells)
            {
                bool isPlace = false;
                if (defCursor < definition.Length && definition.ToLower() != "x")
                {
                    int pos = definition[defCursor] - '1';
                    if (pos == maybePlace)
                    {
                        isPlace = true;
                        ++defCursor;
                    }
                    else if (pos < maybePlace)
                    {
                        throw new Exception("Out of order bell in [" + definition + "]");
                    }
                }
                if (isPlace)
                {
                    mapping[maybePlace] = maybePlace;
                    ++maybePlace;
                }
                else
                {
                    if (maybePlace == noBells - 1)
                    {
                        throw new Exception("Attempt to swap at end with definition  [" + definition + "]");
                    }
                    mapping[maybePlace] = maybePlace + 1;
                    mapping[maybePlace + 1] = maybePlace;
                    maybePlace += 2;
                }
            }
            var ret = Permutation.GetPermutation(mapping);
            permutationNames[ret] = definition;
            return ret;
            
        }

        public override string ToString()
        {
            string ret;
            if ( !permutationNames.TryGetValue(this,out ret))
            {
                ret = string.Join("-", mapping);
            }
            return ret;
        }
        static  Dictionary<Permutation, string> permutationNames = new Dictionary<Permutation, string>();

        /// <summary>
        /// Create a permutation from a single definition in the form of a set of bells to rotate within a change
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="noBells"></param>
        public static Permutation FromRotation(int firstPlaceInRotation, int bellsInRotation, int noBells)
        {
            int[] mapping = new int[noBells];
            for (int i = 0; i < firstPlaceInRotation; ++i)
            {
                mapping[i] = i;
            }
            for (int i = 0; i < bellsInRotation; ++i)
            {
                mapping[i + firstPlaceInRotation] = ((i + 1) % bellsInRotation) + firstPlaceInRotation;
            }
            for (int i = firstPlaceInRotation + bellsInRotation; i < noBells; ++i)
            {
                mapping[i] = i;
            }
            return Permutation.GetPermutation(mapping);
        }

        public string Apply(string text)
        {
            return new string(Apply(text.ToCharArray()));
        }


        public int Apply(int number)
        {
            //int n = number / 5040;
            //if (!computed[n])
            //{
            //  for (int i = n * 5040; i < (n + 1) * 5040; ++i)
            //  {
            //    _rowMapping[i] = Row.FromNumber(i).Apply(mapping).ToNumber();
            //  }
            //  computed[n] = true;
            //}
            //lock (_rowMapping)
            {
                int newRow = _rowMapping[number];
                if (newRow < 0)
                {
                    _rowMapping[number] = newRow = Row.FromNumber(number).Apply(mapping).ToNumber();
                }
                return newRow;
            }

        }

        public char[] Apply(char[] text)
        {
            if (text.Length != mapping.Length)
            {
                throw new Exception("Mismatched length. Perm length = " + mapping.Length + ": text length = " + text.Length);
            }
            char[] newChars = new Char[text.Length];
            for (int i = 0; i < mapping.Length; ++i)
            {
                newChars[i] = text[mapping[i]];
            }
            return newChars;
        }

        public static char[] Apply(char[] text, int[] extMapping)
        {
            if (text.Length != extMapping.Length)
            {
                throw new Exception("Mismatched length. Perm length = " + extMapping.Length + ": text length = " + text.Length);
            }
            char[] newChars = new Char[text.Length];
            for (int i = 0; i < extMapping.Length; ++i)
            {
                newChars[i] = text[extMapping[i]];
            }
            return newChars;
        }
    }
}
