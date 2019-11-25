using System;
using System.Collections.Generic;
using System.Threading;

// code : see http://coding-time.blogspot.fr/2008/03/implement-your-own-parallelfor-in-c.html
public static class Parallel
{
   public static void ForAutoChunkSize (int fromInclusive, int toExclusive, Action<int> action)
   {
      For (
         fromInclusive,
         toExclusive,
         action,
         (toExclusive - fromInclusive) / Environment.ProcessorCount + 1,
         Environment.ProcessorCount
         );
   }

   public static void For (int fromInclusive, int toExclusive, Action<int> action)
   {
      For (fromInclusive, toExclusive, action, 4, Environment.ProcessorCount);
   }

   public static void For (int fromInclusive, int toExclusive, Action<int> action, int chunkSize, int threadCount)
   {
      if (chunkSize < 1)
         chunkSize = 1;
      if (threadCount < 1)
         threadCount = 1;

      int index = fromInclusive - chunkSize;
      var locker = new object ();

      Action process = () =>
      {
         int chunkStart;
         while (true)
         {
            chunkStart = 0;
            lock (locker)
               chunkStart = index += chunkSize;
            for (int i = chunkStart; i < chunkStart + chunkSize; i++)
            {
               if (i >= toExclusive)
                  return;
               action (i);
            }
         }
      };

      IAsyncResult[] results = new IAsyncResult[threadCount];
      for (int i = 0; i < threadCount; ++i)
         results[i] = process.BeginInvoke (null, null);

      // wait all
      for (int i = 0; i < threadCount; ++i)
         process.EndInvoke (results[i]);
   }

   public static void Invoke (params Action[] actions)
   {
      IAsyncResult[] results = new IAsyncResult[actions.Length];

      for (int i = 0; i < actions.Length; i++)
         results[i] = actions[i].BeginInvoke (null, null);

      for (int i = 0; i < actions.Length; i++)
         actions[i].EndInvoke (results[i]);
   }
}
