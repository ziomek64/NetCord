﻿using System.Collections;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Commands;

internal class SortedList<T> : ICollection<T>, IReadOnlyList<T>
{
    private T[] _items;
    private readonly Comparison<T> _comparison;
    private int _size;

    public T this[int index] => _items[index];

    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _size)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (value != _items.Length)
            {
                if (value > 0)
                {
                    var newItems = new T[value];
                    if (_size > 0)
                        Array.Copy(_items, newItems, _size);
                    _items = newItems;
                }
                else
                    _items = Array.Empty<T>();
            }
        }
    }

    public SortedList(Comparison<T> comparison)
    {
        _items = [];
        _size = 0;
        _comparison = comparison;
    }

    public int Count => _size;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (_size == _items.Length)
            Grow(_size + 1);
        if (_size == 0)
        {
            _items[0] = item;
            _size++;
        }
        else
        {
            for (var i = 0; i < _size; i++)
            {
                if (_comparison(item, _items[i]) < 0)
                {
                    Array.Copy(_items, i, _items, i + 1, _size - i);
                    _items[i] = item;
                    _size++;
                    return;
                }
            }
            _items[_size++] = item;
        }
    }

    private void Grow(int capacity)
    {
        var newCapacity = _items.Length + 1;

        if ((uint)newCapacity > Array.MaxLength)
            newCapacity = Array.MaxLength;

        if (newCapacity < capacity)
            newCapacity = capacity;

        Capacity = newCapacity;
    }

    public void Clear()
    {
        _items = Array.Empty<T>();
        _size = 0;
    }

    public bool Contains(T item) => IndexOf(item) != -1;

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (index >= _size)
            throw new ArgumentOutOfRangeException(nameof(index));
        _size--;
        if (index < _size)
            Array.Copy(_items, index + 1, _items, index, _items.Length - index);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            _items[index] = default!;
    }

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_items[.._size].GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items[.._size].GetEnumerator();

    public int IndexOf(T item) => Array.IndexOf(_items, item);
}
