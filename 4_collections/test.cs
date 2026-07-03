using System;
using System.Collections;
using System.Collections.Generic;

public class SmartStack<T> : IEnumerable<T>
{
    // Внутренний массив для хранения элементов стека.
    private T[] _items;
    // Текущее количество элементов в стеке.
    private int _count;

    // Свойство Count - получение количества элементов в стеке.
    public int Count
    {
        get { return _count; }
    }
    // Свойство Capacity - получение ёмкости.
    public int Capacity
    {
        get { return _items.Length; }
    }

    // Конструктор без параметров.
    public SmartStack() : this(4)
    {
    }

    // Конструктор с одним целочисленным параметром.
    public SmartStack(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity),
             "Начальная ёмкость не может быть отрицательной.");
        }
        _items = new T[capacity];
        _count = 0;
    }

    // Конструктор, который в качестве параметра принимает
    // коллекцию, реализующую интерфейс IEnumerable<T>.
    public SmartStack(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
        List<T> tempList = new List<T>(collection);
        
        _items = new T[Math.Max(tempList.Count, 4)];
        _count = 0;

        foreach (T item in tempList)
        {
            Push(item);
        }
    }

    // Метод Push, добавляющий элемент на вершину стека.
    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            int newCapacity = (_items.Length == 0) ? 4 : 
            _items.Length * 2;
            Array.Resize(ref _items, newCapacity);
        }
        _items[_count] = item;
        _count++;
    }

    // Метод PushRange, добавляющий на вершину стека 
    // содержимое коллекции,реализующей интерфейс IEnumerable<T>.
    public void PushRange(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
        foreach (T item in collection)
        {
            Push(item);
        }
    }

    // Метод Pop, удаляющий и возвращающий элемент с вершины стека.
    public T Pop()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException(@"Стек пуст.
             Невозможно выполнить Pop.");
        }

        _count--;
        T item = _items[_count];
        _items[_count] = default(T);
        return item;
    }

    // Метод Peek, возвращающий элемент 
    //с вершины стека без его удаления.
    public T Peek()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException(@"Стек пуст.
             Невозможно выполнить Peek.");
        }
        return _items[_count - 1];
    }

    // Метод Contains, проверяющий наличие элемента в стеке.
    public bool Contains(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    // Индексатор, позволяющий работать с элементом по глубине.
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                 "Индекс находится вне диапазона стека.");
            }
            return _items[_count - 1 - index];
        }
        set
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                 "Индекс находится вне диапазона стека.");
            }
            _items[_count - 1 - index] = value;
        }
    }

    // Методы, реализующие интерфейсы IEnumerable и IEnumerable<T>.
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = _count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    // Явная реализация IEnumerable.
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class SmartStackDemo
{
    public static void Main(string[] args)
    {
        Console.WriteLine("\nДемонстрация SmartStack<int>\n");

        // 1. Конструктор без параметров.
        Console.WriteLine(@"Создаем стек с конструктором 
        без параметров (емкость 4):");
        SmartStack<int> stack1 = new SmartStack<int>();
        Console.WriteLine($@"Count: {stack1.Count}, 
        Capacity: {stack1.Capacity}");

        stack1.Push(10);
        stack1.Push(20);
        stack1.Push(30);
        stack1.Push(40);
        Console.WriteLine($@"После 4 Push: Count: {stack1.Count},
         Capacity: {stack1.Capacity}"); 
        stack1.Push(50);
        Console.WriteLine($@"После 5 Push (удвоение): Count:
         {stack1.Count}, Capacity: {stack1.Capacity}");
        Console.WriteLine($@"Вершина стека (Peek):
         {stack1.Peek()}"); 

        // 2. Конструктор с указанной емкостью.
        Console.WriteLine("\nСоздаем стек с конструктором:");
        SmartStack<string> stack2 = new SmartStack<string>(2);
        Console.WriteLine($@"Count: {stack2.Count}, 
        Capacity: {stack2.Capacity}");
        stack2.Push("Один");
        stack2.Push("Два");
        stack2.Push("Три");
        Console.WriteLine($@"После 3 Push: Count: 
        {stack2.Count}, Capacity: {stack2.Capacity}");

        // 3. Конструктор с коллекцией IEnumerable<T>.
        Console.WriteLine("\nСоздаем стек из коллекции IEnumerable<int>:");
        List<int> initialNumbers = new List<int> { 100, 200, 300, 400 };
        SmartStack<int> stack3 = new SmartStack<int>(initialNumbers);
        Console.WriteLine($@"Count: {stack3.Count},
         Capacity: {stack3.Capacity}");
        Console.WriteLine($"Вершина стека: {stack3.Peek()}");

        // 4. Pop и обработка исключений.
        Console.WriteLine("\nДемонстрация Pop:");
        Console.WriteLine($"Pop: {stack3.Pop()}"); 
        Console.WriteLine($"Pop: {stack3.Pop()}"); 
        Console.WriteLine($"Count: {stack3.Count}"); 
        
        try
        {
            SmartStack<int> emptyStack = new SmartStack<int>();
            emptyStack.Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($@"Ожидаемая ошибка при
             Pop из пустого стека: {ex.Message}");
        }

        // 5. PushRange.
        Console.WriteLine("\nДемонстрация PushRange:");
        List<string> moreItems = new List<string> { "Apple", 
        "Banana", "Cherry" };
        stack2.PushRange(moreItems);
        Console.WriteLine($@"После PushRange: Count: {stack2.Count},
         Capacity: {stack2.Capacity}"); 
        Console.WriteLine($"Вершина стека: {stack2.Peek()}");

        // 6. Contains.
        Console.WriteLine("\nДемонстрация Contains:");
        Console.WriteLine($@"stack2 содержит 'Banana': 
        {stack2.Contains("Banana")}");
        Console.WriteLine($@"stack2 содержит 'Grape': 
        {stack2.Contains("Grape")}");

        // 7. Обход стека с помощью foreach.
        Console.WriteLine("\nОбход stack2:");
        foreach (string item in stack2)
        {
            Console.WriteLine(item);
        }

        // 8. Индексатор.
        Console.WriteLine("\nДемонстрация индексатора:");
        Console.WriteLine($"Вершина (индекс 0): {stack2[0]}");
        Console.WriteLine($"Основание (индекс Count-1): {stack2[stack2.Count - 1]}");
        stack2[0] = "New Cherry";
        Console.WriteLine($"Новая вершина: {stack2.Peek()}");

        try
        {
            Console.WriteLine(stack2[stack2.Count]);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($@"Ожидаемая ошибка
             индексатора: {ex.Message}");
        }
    }
}