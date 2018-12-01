import java.time.Duration;
import java.time.Instant;
import java.util.Random;
import java.util.concurrent.*;


public class Main {
    private static final int maxNumber = 10000000;

    private static ExecutorService es;

    private static int size = 5000000;
    private static int[] vec;
    private static int[] copy;

    public static void main(String[] args){
        String alg = "q";
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
                ParallelMerge(0,size-1,100000);
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

    public static void ParallelMerge(int l, int r, int d)
    {
        es = Executors.newCachedThreadPool();
        ParallelMergeInternal(l,r,d);
        es.shutdown();
        try {
            if(!es.awaitTermination(1,TimeUnit.MINUTES)){
                es.shutdownNow();
            }
        } catch (InterruptedException e) {
            e.printStackTrace();
            es.shutdownNow();
            Thread.currentThread().interrupt();
        }
    }


    public static void ParallelMergeInternal(int l, int r, int d) {
       // if ( r <= l ) return;
        if (r - l +1<= d)
        {
            QuickSort(l, r);
            return;
        }
        int m = (l + r) / 2;
        Future f1 = es.submit((Callable<Boolean>) () -> {
            ParallelMergeInternal(l, m, d);
            return true;
        });
        Future f2 = es.submit((Callable<Boolean>) () -> {
            ParallelMergeInternal(m + 1, r, d);;
            return true;
        });

        try {
            f1.get();
            f2.get();
        } catch (InterruptedException e) {
            e.printStackTrace();
        } catch (ExecutionException e) {
            e.printStackTrace();
        }
        InLineMerge(l, m, r);
    }

    private static void InLineMerge(int left ,int middle,int right){
        int length1 = middle - left + 1;
        int length2 = right - middle;
        if ( length1 >= length2 )
        {
            if ( length2 <= 0 )  return;
            if (( length1 + length2 ) < 1000 ){
                Merge(left,middle,middle+1,right);
                return;
            }
            int q1 = ( left + middle ) / 2;
            int q2 = BinSearch( vec[q1], middle + 1, right);
            int q3 = q1 + ( q2 - middle - 1 );
            BlockExchangeMirror( q1, middle, q2 - 1 );
            InLineMerge( left,q1 - 1,q3 - 1 );
            InLineMerge(q3 + 1,q2 - 1,right);
        }
        else {  // length1 < length2
            if ( length1 <= 0 )  return;
            if (( length1 + length2 ) < 1000 ){
                Merge(left,middle,middle+1,right);
                return;
            }
            int q1 = ( middle + 1 + right ) / 2;
            int q2 = BinSearch( vec[ q1 ], left, middle);
            int q3 = q2 + ( q1 - middle - 1 );
            BlockExchangeMirror(q2,middle,q1);
            InLineMerge(left,q2 - 1,q3 - 1 );
            InLineMerge(q3 + 1, q1,right);
        }
    }

    private static int BinSearch(int value, int left, int right){
        int low  = left;
        int high = Math.max( left, right+1);
        while( low < high )
        {
            int mid = ( low + high ) / 2;
            if ( value <= vec[ mid ] ) {
                high = mid;
            } else {
                low  = mid + 1;
            }
        }
        return high;
    }

    private static void BlockExchangeMirror(int left, int middle, int right){
        Mirror( left, middle );
        Mirror(middle + 1, right );
        Mirror( left, right );
    }

    private static void Mirror(int left, int right) {
        while (left < right) {
            Swap(left++, right--);
        }
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

    public static boolean CheckVec(){
        for (int i = 0; i < size - 1; i++) {
            if (vec[i] > vec[i+1]) return false;
        }
        return true;
    }
}
