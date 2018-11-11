import java.time.Duration;
import java.time.Instant;
import java.util.Random;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;


public class Main {
    private static final int maxNumber = 10000000;

    private static int size = 70;
    private static int[] vec;
    private static int[] copy;

    public static void main(String[] args){
        String alg = "m";
        if (args.length > 1) alg = args[1];
        InitVec(args);
        Instant start;
        Instant finish;

        switch (alg){
            case "q" :
                start = Instant.now();
                QuickSort(0, size - 1);
                finish = Instant.now();
                ResultPrint("QiuckSort",Duration.between(start, finish).toMillis(),size);
                break;
            case "m" :
                start = Instant.now();
                ParallelMerge(0,size-1,16);
                finish = Instant.now();
                ResultPrint("ParallelMerge",Duration.between(start, finish).toMillis(),size);
                break;
             default:
                 System.out.println("ELső paraméter: tömb mérete Második paraméter: algoritmus q - quicksort/m - parallel merge");
                 break;
        }
    }

    public static void ResultPrint(String alg, long ms, int size){
        System.out.println(alg+';'+ms+';'+size);
    }



    public static void ParallelMerge(int l, int r,int d)
    {
        ExecutorService es = Executors.newCachedThreadPool();
        if (r - l +1< d)
        {
            QuickSort(l, r);
            return;
        }
        int m = (l + r) / 2;
        es.execute(new Runnable() {
            @Override
            public void run() {
                ParallelMerge(l, m,d);
            }
        });
        es.execute(new Runnable() {
            @Override
            public void run() {
                ParallelMerge(m + 1, r,d);
            }
        });
        es.shutdown();
        try {
            if(!es.awaitTermination(3,TimeUnit.MINUTES)){
                es.shutdownNow();
            }
        } catch (InterruptedException e) {
            e.printStackTrace();
            es.shutdownNow();
            Thread.currentThread().interrupt();
        }


        Merge(l, m,m+1, r);
    }

    private static void Merge(int l1, int r1, int l2, int r2)
    {
        int[] temp = new int[r2 - l1 + 1];
        int index = 0;
        int start = l1;
        while (l1 <= r1 && l2 <= r2)
        {
            if (vec[l1] <= vec[l2])
            {
                temp[index++] = vec[l1++];
            }
            else
            {
                temp[index++] = vec[l2++];
            }
        }
        while (l1 <= r1)
        {
            temp[index++] = vec[l1++];
        }
        while (l2 <= r2)
        {
            temp[index++] = vec[l2++];
        }
        index = 0;
        for (int i = start; i < l2; i++)
        {
            vec[i] = temp[index++];
        }
    }

    public static void QuickSort(int left, int right)
    {
        QuickSortInternal(left, right);
    }

    private static void QuickSortInternal(int left, int right)
    {
        if (left >= right) return;

        int last = left;
        for (int k = left; k <= right - 1; k++)
        {
            if (vec[k] < vec[right])
            {
                Swap(last, k);
                last++;
            }
        }
        Swap(last, right);

        QuickSortInternal(left, last - 1);
        QuickSortInternal(last + 1, right);

    }

    private static void Swap(int i, int j)
    {
        int temp = vec[i];
        vec[i] = vec[j];
        vec[j] = temp;
    }

    public static void InitVec(String[] args){
        Random random = new Random();
        if (args.length > 0) {
            size = Integer.parseInt(args[0]);
        }
        vec = new int[size];
        copy = new int[size];
        for (int i = 0; i < size; i++) {
                vec[i] = random.nextInt(maxNumber)+1;
                copy[i] = vec[i];
        }
    }

    public static void ResetVec(){
        for (int i = 0; i < size; i++) {
            vec[i] = copy[i];
        }
    }

    public static void PrintOut(){
        System.out.println();
        for (int i = 0; i < size; i++) {
                System.out.print(vec[i] + "\t");
        }
        System.out.println();
    }
}
