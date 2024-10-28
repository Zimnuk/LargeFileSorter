The chronology of hypotheses for solving the problem
1. My first idea is based on the merge sort method. But with some changes in implementation, to save time of development. First, we will split the large file into several smaller files, each comparable in size to the available memory. We will sort each of them and then merge the sorted files into one. For the merging process, I plan to use a data structure like a PriorityQueue, which will allow us to utilize a built-in algorithm for comparing multiple elements at once.
2. In first version aggregation of sorted result and write all that text to file wasted too much time. After fixing it with line by line writing using StreamWriter, sorting of 1GB file spending over 50 seconds, 10GB over 9 minutes.
