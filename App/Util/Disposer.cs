using System;
using System.Collections.Generic;
using System.Text;

namespace App.Util {

  public class Disposer : IDisposable {

    private List<IDisposable> Items { get; init; } = new();

    public T Add<T>(T item) where T : class, IDisposable {
      Items.Add(item);
      return item;
    }

    public T Add<T>(Func<T> fn) where T : class, IDisposable {
      T item = fn();
      if (item != null)
        Add(item);
      return item;
    }

    public void Dispose() {
      foreach (var item in Items) {
        if (item == null)
          continue;
        item.Dispose();
      }
      Items.Clear();
    }

  }

}
