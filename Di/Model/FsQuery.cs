//  
//  FsQuery.cs
//  
//  Author:
//       Karl Voelker <ktvoelker@gmail.com>
// 
//  Copyright (c) 2011 Karl Voelker
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Di.Model
{
	public class Fs
	{
		public static readonly FsCache<Karl.Fs.File, File> File = new FsCache<Karl.Fs.File, File>((m, i) => new File(42, m, i));
		
		public static readonly FsCache<Karl.Fs.Directory, Directory> Directory = new FsCache<Karl.Fs.Directory, Directory>((m, i) => new Directory(42, m, i));
	}
	
	public class FsCache<K, V> where K : Karl.Fs.Entry where V : FsQueryable<K>
	{
		private readonly IDictionary<Main, IDictionary<K, V>> Items = new Dictionary<Main, IDictionary<K, V>>();
		
		private readonly Func<Main, K, V> Create;
		
		public FsCache(Func<Main, K, V> create)
		{
			Create = create;
		}
		
        public V Get(Main root, K info)
        {
            if (!Items.ContainsKey(root))
            {
                Items[root] = new Dictionary<K, V>();
            }
            if (!Items[root].ContainsKey(info))
            {
                Items[root][info] = Create(root, info);
            }
            return Items[root][info];
        }

        public IEnumerable<V> GetAll(Main root)
        {
            return Items[root].Values;
        }
	}
	
	public interface IFsQueryable
	{
		Main Root
		{
			get;
		}
		
		Directory Parent
		{
			get;
		}
		
		Language.Base Lang
		{
			get;
		}
		
		string Name
		{
			get;
		}
		
		string FullName
		{
			get;
		}
	}
	
    public abstract class FsQueryable<I> : IFsQueryable where I : Karl.Fs.Entry
    {
        public Main Root
		{
			get;
			private set;
		}
		
		public readonly I Info;
		
		public FsQueryable(Main root, I info)
		{
			Root = root;
			Info = info;
		}

        public Directory Parent
        {
            get;
			protected set;
        }

        public Language.Base Lang
        {
            get;
			protected set;
        }

        public string Name
        {
            get
			{
				return Info.Name;
			}
        }

        public string FullName
        {
            get
			{
				return Info.FullName;
			}
        }
    }

    public class FsQuery<T> where T : IFsQueryable
    {
        private string query;

        public FsQuery(string _query)
        {
            query = _query;
        }

        public IQueryable<T> Evaluate(IEnumerable<T> files)
        {
            return files.AsQueryable().Where(f => f.Name == query);
        }
    }
}

