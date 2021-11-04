﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoNewsWebsite.DAL
{
    public static class Handler
    {
        public static T GetFromIndex<T>(Guid index) where T : new()
        {
            var result = Engine.Select($"SELECT * FROM {typeof(T).Name} WHERE Id = '{index}'");
            return CreateObjectFromIndex<T>(result, 0);
        }

        private static T CreateObjectFromIndex<T>(SelectResult result, int index) where T : new()
        {
            var item = new T();
            for (int i = 0; i < result.ColumnsCount; i++)
            {
                typeof(T).GetProperty(result.Header[i])?.SetValue(item, result.Data[index][i]);
            }
            return item;
        }

        public static List<T> GetAll<T>() where T : new()
        {
            var result = Engine.Select($"SELECT * FROM {typeof(T).Name}");
            var list = new List<T>();
            for (int i = 0; i < result.RowCount; i++)
            {
                list.Add(CreateObjectFromIndex<T>(result, i));
            }

            return list;
        }

        private static object GetValueFromProperty<T>(string header, T item)
        {
            return typeof(T).GetProperty(header)?.GetValue(item);
        }
        
        public static void Insert<T>(this T item)
        {
            var headers = typeof(T).GetProperties().Select(i => i.Name).ToList();
            var values = headers.Select(value => GetValueFromProperty(value, item)).ToList();

            Engine.Insert(
                $"INSERT INTO [{typeof(T).Name}] ({String.Join(", ", headers)}) " +
                $"VALUES ('{String.Join("', \'", values)}')"
                );
        }

        public static void Delete<T>(Guid id)
        {
            Engine.Delete($"DELETE FROM [{typeof(T).Name}] " +
                          $"WHERE Id='{id}'");
        }

        public static void Update<T>(Guid id, T item)
        {
            var header = typeof(T).GetProperties().Select(i => i.Name).ToList(); //получить список параметров
            var values = 
                header.Select(value => $"{value}=\'{GetValueFromProperty(value, item)}\'").ToList(); //получить список значений
            
            Engine.Update(
                $"UPDATE [{typeof(T).Name}] " +
                          $"SET {String.Join(", ", values)} " +
                          $"WHERE Id='{GetValueFromProperty("Id", item)}'"
                );
        }
    }
}