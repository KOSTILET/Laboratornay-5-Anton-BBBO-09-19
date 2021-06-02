using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp5
{
  class Program
  {
    static void Main(string[] args)
    {
      int choice = 0;
      Console.WriteLine("Состояние памяти -  1");
      Console.WriteLine("Состояние очереди - 2");
      Console.WriteLine("Добавить задачи -   3");
      Console.WriteLine("Убрать задачу -     4");
      Console.WriteLine("Выход -             0");
      do
      {
        choice = Convert.ToInt32(Console.ReadLine());
        switch (choice)
        {
          case 1:
            {
              MemoryInfo();
            }
            break;
          case 2:
            {
              QueueInfo();
            }
            break;
          case 3:
            {
              lock (locker)
              {
                Thread myThread = new Thread(ProccesAdd);
                myThread.Start();
              }
            }
            break;
          case 4:
            {
              ProccesDelete();
            }
            break;
          default:
            Console.WriteLine("! Введите правильные данные !");
            break;
        }
      } while (choice != 0);
    }
    static void CreateFile(string name, int size)
    {
      using (FileStream fWrite = new FileStream(name, FileMode.Create, FileAccess.Write))
      {
        fWrite.SetLength(size);
      }

      COUNT++;
    }
    static void ProccesAdd()
    {
      int sizeOfFile, count = 0;
      string fileName = $"{COUNT + 1}.txt";
      sizeOfFile = random.Next(65535);
      CreateFile(fileName, sizeOfFile);
      foreach (int m in MEMORY)
      {
        count += m;
      }

      if ((count + sizeOfFile) <= MEMORY_SIZE)
      {
        MEMORY.Enqueue(sizeOfFile);
        Console.WriteLine("Задача добавлена!");
      }
      else
      {
        QUEUE.Enqueue(sizeOfFile);
        Console.WriteLine("В пямяти не хватило места! Перенесен в очередь!");
      }
      var outer = Task.Factory.StartNew(() =>
      {
        ProccesDo();
        var inner = Task.Factory.StartNew(() =>
        {
          QueueDo();
        }, TaskCreationOptions.AttachedToParent);
      });
      outer.Wait();
    }
    static void QueueDo()
    {
      int count = 0;
      if (MEMORY.Count == 0 && QUEUE.Count == 0)
      {
        Console.WriteLine("НЕТ ЗАДАЧ !");
      }

      foreach (int m in MEMORY)
      {
        count += m;
      }
      if (QUEUE.Count != 0)
      {
        if ((count + QUEUE.Peek()) <= MEMORY_SIZE)
        {
          MEMORY.Enqueue(QUEUE.Dequeue());
        }
      }
    }

    static void MemoryInfo()
    {
      int occupiedMemory = 0;
      foreach (int m in MEMORY)
      {
        occupiedMemory += m;
      }
      Console.WriteLine($"Задач: {MEMORY.Count} | Свободно: {MEMORY_SIZE - occupiedMemory} | Занято: {occupiedMemory}");
    }
    static void QueueInfo()
    {
      Console.WriteLine($"Задач в очереди: {QUEUE.Count}");
    }

    static void ProccesDo()
    {
      Random rand = new Random();
      Console.WriteLine("Задача делает работу");
      Thread.Sleep(rand.Next(2000, 5000));
      if (MEMORY.Count != 0)
      {
        MEMORY.Dequeue();
        Console.WriteLine("Задача удалена");
      }
    }
    static void ProccesDelete()
    {
      int choice;
      Console.WriteLine($"Задач в памяти: {MEMORY.Count}");
      if (MEMORY.Count == 0)
      {
        Console.WriteLine("Вы ничего не можете удалить !");
        return;
      }
      else
      {
        Console.WriteLine("Какую задачу удалить? ");
        choice = Convert.ToInt32(Console.ReadLine());
        int queueCount = MEMORY.Count;
        for (int i = 0; i < queueCount; i++)
        {
          int currentFirstElement = MEMORY.Dequeue();

          if (choice != currentFirstElement)
          {
            MEMORY.Enqueue(currentFirstElement);
          }
        }
      }
    }
    static int COUNT = 0;
    public const int MEMORY_SIZE = 64000;
    static Queue<int> MEMORY = new Queue<int>();
    static Queue<int> QUEUE = new Queue<int>();
    static object locker = new object();
    public static Random random = new Random();
    
  }
}