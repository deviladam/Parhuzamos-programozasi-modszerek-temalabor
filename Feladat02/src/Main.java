import java.time.Duration;
import java.time.Instant;
import java.util.Random;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;


public class Main {
    private static final int rngMax = 9;

    private static int[][] mxA;
    private static int[][] mxB;
    private static int[][] mxAB;
    private static int size = 111;

    public static void main(String[] args){
        String alg = "\\all";
        if (args.length > 1) alg = args[1];
        Init(args);
        Instant start;
        Instant finish;

        switch (alg){
            case "\\a" :
                start = Instant.now();
                SimpleRow();
                finish = Instant.now();
                ResultPrint("NaivRow",Duration.between(start, finish).toMillis(),size);
                break;
            case "\\b" :
                start = Instant.now();
                SimpleColum();
                finish = Instant.now();
                ResultPrint("NaivColum",Duration.between(start, finish).toMillis(),size);
                break;
            case "\\c" :
                start = Instant.now();
                FixedThreadPool(args);
                finish = Instant.now();
                ResultPrint("FixedThreadPool",Duration.between(start, finish).toMillis(),size);
                break;
            case "\\d" :
                start = Instant.now();
                CachedThreadPool();
                finish = Instant.now();
                ResultPrint("CachedThreadPool",Duration.between(start, finish).toMillis(),size);
                break;
            case "\\all":
                long[] times = new long[4];

                start = Instant.now();
                SimpleRow();
                finish = Instant.now();
                times[0] = Duration.between(start, finish).toMillis();

                ResetMxAB();

                start = Instant.now();
                SimpleColum();
                finish = Instant.now();
                times[1] = Duration.between(start, finish).toMillis();

                ResetMxAB();

                start = Instant.now();
                FixedThreadPool(args);
                finish = Instant.now();
                times[2] = Duration.between(start, finish).toMillis();

                ResetMxAB();

                start = Instant.now();
                CachedThreadPool();
                finish = Instant.now();
                times[3] = Duration.between(start, finish).toMillis();

                System.out.println(size+";"+times[0]+';'+times[1]+';'+times[2]+';'+times[3]);
                break;
             default:
                 System.out.println("Invalid parameter! 1. param: mátrix méret\t2. param: algoritmus (\\a, \\b, \\c, vagy \\d\t{3. param: max threadek száma a ThreadPoolos algoritmushoz}");
                 break;

        }

    }

    public static void ResultPrint(String alg, long ms, int size){
        System.out.println(alg+';'+ms+';'+size);
    }

    public static void SimpleRow(){
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                int sum = 0;
                for (int k = 0; k < size; k++) {
                    sum += mxA[i][k] * mxB[k][j];
                }
                mxAB[i][j] = sum;
            }
        }
    }

    public static void SimpleColum(){
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                int sum = 0;
                for (int k = 0; k < size; k++) {
                    sum += mxA[j][k] * mxB[k][i];
                }
                mxAB[j][i] = sum;
            }
        }
    }

    public static void FixedThreadPool(String[] args){
        ExecutorService es;
        if(args.length > 2) {
            int temp = Integer.parseInt(args[2]);
            es = Executors.newFixedThreadPool(temp <= 0 ? 1 : temp);
        } else {
            es = Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors());
        }

        for (int i = 0; i < size; i++) {
            int j = i;
            es.execute(new Runnable() {
                @Override
                public void run() {
                    SumColum(j);
                }
            });
        }
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
    }

    public static void CachedThreadPool(){
        ExecutorService es = Executors.newCachedThreadPool();
        for (int i = 0; i < size; i++) {
            int j = i;
            es.execute(new Runnable() {
                @Override
                public void run() {
                    SumColum(j);
                }
            });
        }
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
    }


    public static void SumColum(int j){
        for (int i = 0; i < size; i++)
        {
            int sum = 0;
            for (int k = 0; k < size; k++)
            {
                sum += mxA[i][k] * mxB[k][j];
            }
            mxAB[i][j] = sum;
        }
    }

    public static void Init(String[] args){
        Random random = new Random();
        if (args.length > 0) {
            size = Integer.parseInt(args[0]);
        }
        mxA = new int[size][size];
        mxB = new int[size][size];
        mxAB = new int[size][size];
        ResetMxAB();
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                mxA[i][j] = random.nextInt(rngMax)+1;
                mxB[i][j] = random.nextInt(rngMax)+1;

            }
        }
    }

    public static void ResetMxAB(){
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                mxAB[i][j] = 0;
            }
        }
    }

    public static void PrintOut(){
        System.out.println();
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                System.out.print(mxAB[i][j] + "\t");
            }
            System.out.println();
        }
    }
}
