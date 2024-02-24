TimeSharingOSModel os = new TimeSharingOSModel();
int ioTasks = 10, calcTasks = 15, balancedTasks = 25;
os.CreateTasksPackage(ioTasks, calcTasks, balancedTasks);
int operations, time;
(operations, time) = os.ExecuteTaskPackage();
int allTasksCount = ioTasks + calcTasks + balancedTasks;
Console.WriteLine($"Tasks / 1000t.u. : {(allTasksCount * 1000.0) / time}");
Console.WriteLine($"Operations / 1000t.u. : {(operations * 1000.0) / time}");



class Task
{
    protected static int timeCalcOperation = 10;
    protected static int timeIOutOperation = 100;
    protected static int quantum = 10;
    protected static int quantumWithLoss = 11;
    public int QuantumAmount { get; set; } = 0;
    public int AllQuantums { get; set; } = 0;
    public virtual int GetTime() { return 0; }
    public virtual int GetOperations() { return 0; }
    public virtual string GetTaskName() { return ""; }
}
class InOutputTask : Task
{
    protected int amountIOOperations;
    public static string name = "InOutputTask";
    public InOutputTask(int amountIOOperations)
    {
        this.amountIOOperations = amountIOOperations;
        QuantumAmount = (int)Math.Ceiling((double)GetTime() / quantum);
        AllQuantums = (int)Math.Ceiling((double)GetTime() / quantum);
    }
    public override int GetTime()
    {
        return amountIOOperations * timeIOutOperation;
    }
    public override int GetOperations()
    {
        return amountIOOperations;
    }
    public override string GetTaskName()
    {
        return InOutputTask.name;
    }
}
class CalculatingTask : Task
{
    protected int amountCalcOperations;
    public static string name = "CalculatingTask";
    public CalculatingTask(int amountCalcOperations)
    {
        this.amountCalcOperations = amountCalcOperations;
        QuantumAmount = (int)Math.Ceiling((double)GetTime() / quantum);
        AllQuantums = (int)Math.Ceiling((double)GetTime() / quantum);
    }
    public override int GetTime()
    {
        return amountCalcOperations * timeCalcOperation;
    }
    public override int GetOperations()
    {
        return amountCalcOperations;
    }
    public override string GetTaskName()
    {
        return CalculatingTask.name;
    }
}
class BalancedTask : Task
{
    protected int amountIOOperations;
    protected int amountCalcOperations;
    public static string name = "BalancedTask";
    public BalancedTask(int amountIOOperations, int amountCalcOperations)
    {
        this.amountIOOperations = amountIOOperations;
        this.amountCalcOperations = amountCalcOperations;
        QuantumAmount = (int)Math.Ceiling((double)GetTime() / quantum);
        AllQuantums = (int)Math.Ceiling((double)GetTime() / quantum);
    }
    public override int GetTime()
    {
        return amountIOOperations * timeIOutOperation + amountCalcOperations * timeCalcOperation;
    }
    public override int GetOperations()
    {
        return amountIOOperations + amountCalcOperations;
    }
    public override string GetTaskName()
    {
        return BalancedTask.name;
    }
}

class TimeSharingOSModel
{
    protected Queue<Task> tasks = new Queue<Task>();

    public TimeSharingOSModel()
    {
        tasks = new Queue<Task>();
    }

    public void CreateTasksPackage(int countIOTasks, int countCalculatingTasks, int countBalancedTasks)
    {
        Random random = new Random();
        Console.WriteLine($"Package consists of {countIOTasks} IO tasks, {countCalculatingTasks} calculating tasks and {countBalancedTasks} balanced tasks");
        while (countIOTasks > 0 || countCalculatingTasks > 0 || countBalancedTasks > 0)
        {
            int taskType = random.Next(1, 4);

            switch (taskType)
            {
                case 1:
                    if (countIOTasks > 0)
                    {
                        tasks.Enqueue(new InOutputTask(random.Next(1, 6)));
                        countIOTasks--;
                    }
                    break;
                case 2:
                    if (countCalculatingTasks > 0)
                    {
                        tasks.Enqueue(new CalculatingTask(random.Next(1, 21)));
                        countCalculatingTasks--;
                    }
                    break;
                case 3:
                    if (countBalancedTasks > 0)
                    {
                        tasks.Enqueue(new BalancedTask(random.Next(1, 5), random.Next(10, 20)));
                        countBalancedTasks--;
                    }
                    break;
            }
        }
    }

    public (int, int) ExecuteTaskPackage()
    {
        int countAllOperations = 0;
        int countAllTime = 0;
        int executeTimeIOTasks = 0;
        int executeTimeCalcTasks = 0;
        int executeTimeBalancedTasks = 0;
        int executeAllQuantums = 0;
        while (tasks.Count > 0)
        {
            var task = tasks.Dequeue();
            executeAllQuantums++;
            task.QuantumAmount--;
            //Console.WriteLine($"{task.AllQuantums - task.QuantumAmount}/{task.AllQuantums} task in progress");
            if (task.QuantumAmount == 0)
            {
                int taskOperations = task.GetOperations();
                countAllOperations += taskOperations;
                int taskTime = task.GetTime();
                countAllTime += taskTime;
                if (task is InOutputTask)
                {
                    executeTimeIOTasks += taskTime;
                }
                else if (task is CalculatingTask)
                {
                    executeTimeCalcTasks += taskTime;

                }
                else if (task is BalancedTask)
                {
                    executeTimeBalancedTasks += taskTime;
                }
                Console.WriteLine($"A {task.GetTaskName()} with {taskOperations} operations took {executeAllQuantums} quantums to complete");
                continue;
            }
            tasks.Enqueue(task);

        }
        double timeWithLoss = countAllTime * 1.1;
        Console.WriteLine($"{countAllOperations} operations has comleted in {timeWithLoss.ToString("F0")}");
        Console.WriteLine($"IO tasks in {(executeTimeIOTasks * 1.1).ToString("F0")} t.u.");
        Console.WriteLine($"Calculating tasks in {(executeTimeCalcTasks * 1.1).ToString("F0")} t.u.");
        Console.WriteLine($"Balanced tasks in {(executeTimeBalancedTasks * 1.1).ToString("F0")} t.u.");

        return (countAllOperations, (int)timeWithLoss);
    }
}