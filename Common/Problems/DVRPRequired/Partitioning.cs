﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Problems.DVRPRequired
{
    public static class Partitioning
    {
        public static IEnumerable<T[][]> GetAllPartitions<T>(T[] elements)
        {
            return GetAllPartitions(new T[][] { }, elements);
        }
        private static IEnumerable<T[][]> GetAllPartitions<T>(
            T[][] fixedParts, T[] suffixElements)
        {
            // A trivial partition consists of the fixed parts
            // followed by all suffix elements as one block
            yield return fixedParts.Concat(new[] { suffixElements }).ToArray();
            // Get all two-group-partitions of the suffix elements
            // and sub-divide them recursively
            var suffixPartitions = GetTuplePartitions(suffixElements);
            foreach (Tuple<T[], T[]> suffixPartition in suffixPartitions)
            {
                var subPartitions = GetAllPartitions(
                    fixedParts.Concat(new[] { suffixPartition.Item1 }).ToArray(),
                    suffixPartition.Item2);
                foreach (var subPartition in subPartitions)
                {
                    yield return subPartition;
                }
            }
        }

        private static IEnumerable<Tuple<T[], T[]>> GetTuplePartitions<T>(
            T[] elements)
        {
            // No result if less than 2 elements
            if (elements.Length < 2) yield break;

            // Generate all 2-part partitions
            for (int pattern = 1; pattern < 1 << (elements.Length - 1); pattern++)
            {
                // Create the two result sets and
                // assign the first element to the first set
                List<T>[] resultSets = {
                    new List<T> { elements[0] }, new List<T>() };
                // Distribute the remaining elements
                for (int index = 1; index < elements.Length; index++)
                {
                    resultSets[(pattern >> (index - 1)) & 1].Add(elements[index]);
                }

                yield return Tuple.Create(
                    resultSets[0].ToArray(), resultSets[1].ToArray());
            }
        }

        public static IEnumerable<T[][]> GetAllPartitions<T>(T[] elements, int limit)
        {
            return GetAllPartitions(new T[][] { }, elements, limit);
        }
        private static IEnumerable<T[][]> GetAllPartitions<T>(
            T[][] fixedParts, T[] suffixElements, int limit)
        {
            // A trivial partition consists of the fixed parts
            // followed by all suffix elements as one block
            var c = fixedParts.Concat(new[] { suffixElements }).ToArray();
            if (c.Count() <= limit)
            {
                yield return c;

                // Get all two-group-partitions of the suffix elements
                // and sub-divide them recursively
                var suffixPartitions = GetTuplePartitions(suffixElements);
                foreach (Tuple<T[], T[]> suffixPartition in suffixPartitions)
                {
                    var subPartitions = GetAllPartitions(
                        fixedParts.Concat(new[] { suffixPartition.Item1 }).ToArray(),
                        suffixPartition.Item2, limit);
                    foreach (var subPartition in subPartitions)
                    {
                        yield return subPartition;
                    }
                }
            }
        }
    }
}