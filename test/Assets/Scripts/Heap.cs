using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> { //Guarantee that T will implement HeapItem

    T[] items; //An array of all items in the heap
    int currentItemCount; //The current number of items
    
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize]; //The maximum size of the heap

    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;// Have each item know where it is in the index
        items[currentItemCount] = item;//Add a new item to the end of the array
        SortUp(item);//A new item shouldn't necessarily be at the end of the array, so use this method to sort it's position in the array 
        currentItemCount++;//A new item is added, so the current item count should go up.
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];//Declare the first item in the array
        currentItemCount--;//Remove an item from the current item count
        items[0] = items[currentItemCount];//The first item in the index is added to the current item count
        items[0].HeapIndex = 0;//the first item's value in the heap index is 0
        SortDown(items[0]);//compare the first item in the array with the following items
        return firstItem;
    }

    public void UpdateItem(T item)//Updates the heap for sorting
    {
        SortUp(item);

    }

    public int Count //A reference to the current item count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);//Checks if an item is already in the heap index
        
    }


    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;//The value of the left child in the heap can be calculated by multiplying it by 2 and adding 1
            int childIndexRight = item.HeapIndex * 2 + 2;//The value of the left child in the heap can be calculated by multiplying it by 2 and adding 2
            int swapIndex = 0;//swapIndex is used to compare the left and right child and see which one has a higher value

            if(childIndexLeft < currentItemCount)//If the child on the left of the parent is less than the current value...
            {
                swapIndex = childIndexLeft;//The item to swap is the left child

                if(childIndexRight < currentItemCount) //If the child on the right on the parent is also less than the current value...
                {
                    if(items[childIndexLeft].CompareTo(items[childIndexRight]) <0)//Compare the left and right children and determine which one has the lowest value.
                    {
                        swapIndex = childIndexRight;//If the right child has a lower value, it takes priority
                    }
                  
                }
                if (item.CompareTo(items[swapIndex]) < 0)//If the current item has a greater value that the item to swap with...
                {
                    Swap(item, items[swapIndex]);//Swap the two item values in the heap.

                }
                else
                {
                    return;

                }
                
            }
            else
            {
                return;
            }
        }

    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1)/2; //The equation to find the value of the parent in the index

        while (true)
        {
            T parentItem = items[parentIndex]; //The parent item is passed into the items index as a parent
            if(item.CompareTo(parentItem) > 0)//Compare the current item to the parent item. If the current item has a higher priority...
            {
                Swap(item, parentItem);//Swap the parentItem and the currentItem
            }
            else
            {
                break;//Otherwise, break out of the loop
            }
            parentIndex = (item.HeapIndex - 1)/2;//Continue looping through the index
        }
    }
    void Swap(T itemA, T itemB)//Swaps the two declared items around.
    {
        items[itemA.HeapIndex] = itemB;//itemA's position in the heap index is swapped with itemB's position
        items[itemB.HeapIndex] = itemA;//itemB's position in the heap index is swapped with itemA's position
        int itemAIndex = itemA.HeapIndex;//A variable to hold the value of itemA in the array
        itemA.HeapIndex = itemB.HeapIndex;//itemA's value becomes itemB's value
        itemB.HeapIndex = itemAIndex;//itemB's value becomes itemA's previous value
    }

}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex//Gets and sets the vaue of each item in the heap
    {
        get;
        set;

    }


}


