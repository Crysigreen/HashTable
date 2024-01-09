using System;
using System.Collections;
using System.Collections.Generic;

public class HashTable<K, V> : IEnumerable<KeyValue<K, V>>
{

    private const int InitialCapacity = 16;
    private const double LoadFactor = 0.75d;

    private LinkedList<KeyValue<K, V>>[] slots;
    private int count;
    private int collisionCount;


    public HashTable()
    {
        this.slots = new LinkedList<KeyValue<K, V>>[InitialCapacity];
        this.count = 0;
        this.collisionCount = 0;
    }

    public HashTable(int capacity)
    {
        this.slots = new LinkedList<KeyValue<K, V>>[capacity];
        this.count = 0;
    }

    public void Add(K key, V value)
    {
        GrowIfNeeded();
        int slotNumber = FindSlotNumber(key);
        if (slots[slotNumber] == null)
        {
            slots[slotNumber] = new LinkedList<KeyValue<K, V>>();
        }
        else
        {
            collisionCount++;  // Увеличиваем счетчик коллизий
        }

        foreach (var item in slots[slotNumber])
        {
            if (item.key.Equals(key))
            {
                throw new ArgumentException("Key already exists in the hashtable");
            }
        }

        slots[slotNumber].AddLast(new KeyValue<K, V>(key, value));
        count++;
    }

    public int GetCollisionCount()
    {
        return collisionCount;
    }

    private int ImprovedGetHashCode(K key)
    {
        unchecked
        {
            int hash = 17; // начальное значение простого числа
            foreach (char c in key.ToString())
            {
                hash = hash * 31 + c; // 31 - еще одно простое число
            }
            return hash;
        }
    }

    private int FindSlotNumber(K key)
    {
        //int hashCode = key.GetHashCode();
        int hashCode = ImprovedGetHashCode(key);
        return Math.Abs(hashCode % slots.Length);
    }

    private void GrowIfNeeded()
    {
        if ((double)count / slots.Length > LoadFactor)
        {
            Grow();
        }
    }

    public double GetLoadFactor()
    {
        return (double)count / slots.Length;
    }

    private void Grow()
    {
        int newCapacity = slots.Length * 2;
        LinkedList<KeyValue<K, V>>[] newSlots = new LinkedList<KeyValue<K, V>>[newCapacity];

        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var item in slot)
                {
                    //int newSlotNumber = item.key.GetHashCode() % newCapacity;
                    int newSlotNumber = FindSlotNumber(item.key);
                    if (newSlots[newSlotNumber] == null)
                    {
                        newSlots[newSlotNumber] = new LinkedList<KeyValue<K, V>>();
                    }

                    newSlots[newSlotNumber].AddLast(item);
                }
            }
        }

        slots = newSlots;
    }

    public int Size()
    {
        return count;
    }

    public int Capacity()
    {
        return slots.Length;
    }

    public IEnumerator<KeyValue<K, V>> GetEnumerator()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var item in slot)
                {
                    yield return item;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool AddOrReplace(K key, V value)
    {
        int slotNumber = FindSlotNumber(key);
        if (slots[slotNumber] == null)
        {
            slots[slotNumber] = new LinkedList<KeyValue<K, V>>();
        }

        foreach (var item in slots[slotNumber])
        {
            if (item.key.Equals(key))
            {
                item.value = value;
                return true;
            }
        }

        slots[slotNumber].AddLast(new KeyValue<K, V>(key, value));
        count++;
        return false;
    }

    public V Get(K key)
    {
        int slotNumber = FindSlotNumber(key);
        if (slots[slotNumber] != null)
        {
            foreach (var item in slots[slotNumber])
            {
                if (item.key.Equals(key))
                {
                    return item.value;
                }
            }
        }

        throw new KeyNotFoundException($"Key '{key}' not found in the hashtable");
    }

    public KeyValue<K, V> Find(K key)
    {
        int slotNumber = FindSlotNumber(key);
        if (slots[slotNumber] != null)
        {
            foreach (var item in slots[slotNumber])
            {
                if (item.key.Equals(key))
                {
                    return item;
                }
            }
        }

        return null;
    }

    public bool ContainsKey(K key)
    {
        return Find(key) != null;
    }

    public bool Remove(K key)
    {
        int slotNumber = FindSlotNumber(key);
        if (slots[slotNumber] != null)
        {
            LinkedListNode<KeyValue<K, V>> currentNode = slots[slotNumber].First;
            while (currentNode != null)
            {
                if (currentNode.Value.key.Equals(key))
                {
                    slots[slotNumber].Remove(currentNode);
                    count--;
                    return true;
                }
                currentNode = currentNode.Next;
            }
        }

        return false;
    }

    public void Clear()
    {
        slots = new LinkedList<KeyValue<K, V>>[InitialCapacity];
        count = 0;
    }

    public IEnumerable<K> Keys()
    {
        List<K> keys = new List<K>();
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var item in slot)
                {
                    keys.Add(item.key);
                }
            }
        }
        return keys;
    }

    public IEnumerable<V> Values()
    {
        List<V> values = new List<V>();
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var item in slot)
                {
                    values.Add(item.value);
                }
            }
        }
        return values;
    }

}

public class KeyValue<K, V>
{
    public K key { get;  set;}
    public V value { get;  set;}

    public KeyValue(K key, V value)
    {
        this.key = key;
        this.value = value;
    }
}


public class Program
{
    static void Main(string[] args)
    {
        HashTable<string, int> Table = new HashTable<string, int>(10001);

        for (int i = 0; i < 10001; i++)
        {
            Console.WriteLine($"Test{i}");
            Console.WriteLine(i);
            Table.Add($"Test{i}", i);

            if (i % 100 == 0)
            {
                Console.WriteLine($"Load factor: {Table.GetLoadFactor()}");
                Console.WriteLine($"Collisions: {Table.GetCollisionCount()}");
            }
        }

       
       


        // Вывод размера и емкости
        Console.WriteLine($"Size: {Table.Size()}");
        Console.WriteLine($"Capacity: {Table.Capacity()}");

        // Проверка существования ключа
        Console.WriteLine($"Contains key 'Test9996': {Table.ContainsKey("Test9996")}");

        // Получение значения по ключу
        Console.WriteLine($"Value for key 'Test6666': {Table.Get("Test6666")}");

        // Замена значения по ключу
        Table.AddOrReplace("two", 22);

        // Вывод всех ключей и значений
        //Console.WriteLine("Keys:");
        //foreach (var key in Table.Keys())
        //{
        //    Console.WriteLine(key);
        //}

        //Console.WriteLine("Values:");
        //foreach (var value in Table.Values())
        //{
        //    Console.WriteLine(value);
        //}

        // Поиск элемента по ключу
        KeyValue<string, int> foundItem = Table.Find("two");
        if (foundItem != null)
        {
            Console.WriteLine($"Found: {foundItem.key} -> {foundItem.value}");
        }
        else
        {
            Console.WriteLine("Item not found.");
        }

        // Удаление элемента по ключу
        Table.Remove("one");

        // Вывод размера после удаления
        Console.WriteLine($"Size after removal: {Table.Size()}");

        // Очистка хеш-таблицы
        Table.Clear();

        // Вывод размера после очистки
        Console.WriteLine($"Size after clear: {Table.Size()}");
    }
}