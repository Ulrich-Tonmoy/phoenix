#pragma once

#include "CommonHeaders.hpp"

namespace phoenix::utl
{
	template<typename T, bool destruct = true> class vector
	{
	public:
		vector() = default;

		constexpr explicit vector(u64 count)
		{
			resize(count);
		}

		constexpr explicit vector(u64 count, const T& value)
		{
			resize(count, value);
		}

		constexpr vector(const vector& o)
		{
			*this = o;
		}

		constexpr vector(vector&& o) :_capacity{ o._capacity }, _size{ o._size }, _data{ o._data }
		{
			o.reset();
		}

		constexpr vector& operator=(const vector& o)
		{
			assert(this != std::addressof(o));
			if (this != std::addressof(o))
			{
				clear();
				reserve(o._size);
				for (auto& item : o)
				{
					emplace_back(item);
				}
				assert(_size == o._size);
			}
			return *this;
		}

		constexpr vector& operator=(vector&& o)
		{
			assert(this != std::addressof(o));
			if (this != std::addressof(o))
			{
				destroy();
				move(o);
			}
			return *this;
		}

		~vector() { destroy(); }

		constexpr void push_back(const T& value)
		{
			emplace_back(value);
		}

		constexpr void push_back(T&& value)
		{
			emplace_back(std::move(value));
		}

		template<typename... params> constexpr decltype(auto) emplace_back(params&&... p)
		{
			if (_size == _capacity)
			{
				reserve(((_capacity + 1) * 3) >> 1);
			}
			assert(_size < _capacity);

			T* const item{ new (std::addressof(_data[_size])) T(std::forward<params>(p)...) };
			++_size;
			return *item;
		}

		constexpr void resize(u64 new_size)
		{
			static_assert(std::is_default_constructible<T>::value, "Type must be default-constructible.");

			if (new_size > _size)
			{
				reserve(new_size);
				while (_size < new_size)
				{
					emplace_back();
				}
			}
			else if (new_size < _size)
			{
				if constexpr (destruct)
				{
					destruct_range(new_size, _size);
				}

				_size = new_size;
			}
			assert(new_size == _size);
		}

		constexpr void resize(u64 new_size, const T& value)
		{
			static_assert(std::is_copy_constructible<T>::value, "Type must be copy-constructible.");

			if (new_size > _size)
			{
				reserve(new_size);
				while (_size < new_size)
				{
					emplace_back(value);
				}
			}
			else if (new_size < _size)
			{
				if constexpr (destruct)
				{
					destruct_range(new_size, _size);
				}

				_size = new_size;
			}
			assert(new_size == _size);
		}

		constexpr void reserve(u64 new_capacity)
		{
			if (new_capacity > _capacity)
			{
				void* new_buffer{ realloc(_data,new_capacity * sizeof(T)) };
				assert(new_buffer);
				if (new_buffer)
				{
					_data = static_cast<T*>(new_buffer);
					_capacity = new_capacity;
				}
			}
		}

		constexpr T* const erase(u64 index)
		{
			assert(_data && index < _size);
			return erase(std::addressof(_data[index]));
		}

		constexpr T* const erase(T* const item)
		{
			assert(_data && item >= std::addressof(_data[0]) && item < std::addressof(_data[_size]));
			if constexpr (destruct) item->~T();
			--_size;
			if (item < std::addressof(_data[_size]))
			{
				memcpy(item, item + 1, (std::addressof(_data[_size]) - item) * sizeof(T));
			}
			return item;
		}

		constexpr T* const erase_unordered(u64 index)
		{
			assert(_data && index < _size);
			return erase_unordered(std::addressof(_data[index]));
		}

		constexpr T* const erase_unordered(T* const item)
		{
			assert(_data && item >= std::addressof(_data[0]) && item < std::addressof(_data[_size]));
			if constexpr (destruct) item->~T();
			--_size;
			if (item < std::addressof(_data[_size]))
			{
				memcpy(item, std::addressof(_data[_size]), sizeof(T));
			}
			return item;
		}

		constexpr void clear()
		{
			if constexpr (destruct)
			{
				destruct_range(0, _size);
			}
			_size = 0;
		}

		constexpr void swap(vector& o)
		{
			if (this != std::addressof(o))
			{
				auto temp(std::move(o));
				o.move(*this);
				move(temp);
			}
		}

		[[nodiscard]] constexpr T* data()
		{
			return _data;
		}

		[[nodiscard]] constexpr T* const data() const
		{
			return _data;
		}

		[[nodiscard]] constexpr bool empty() const
		{
			return _size == 0;
		}

		[[nodiscard]] constexpr u64 size() const
		{
			return _size;
		}

		[[nodiscard]] constexpr u64 capacity() const
		{
			return _capacity;
		}

		[[nodiscard]] constexpr T& operator[](u64 index)
		{
			assert(_data && index < _size);
			return _data[index];
		}

		[[nodiscard]] constexpr const T& operator[](u64 index) const
		{
			assert(_data && index < _size);
			return _data[index];
		}

		[[nodiscard]] constexpr T& front()
		{
			assert(_data && _size);
			return _data[0];
		}

		[[nodiscard]] constexpr const T& front() const
		{
			assert(_data && _size);
			return _data[0];
		}

		[[nodiscard]] constexpr T& back()
		{
			assert(_data && _size);
			return _data[_size - 1];
		}

		[[nodiscard]] constexpr const T& back() const
		{
			assert(_data && _size);
			return _data[_size - 1];
		}

		[[nodiscard]] constexpr T* begin()
		{
			assert(_data);
			return std::addressof(_data[0]);
		}

		[[nodiscard]] constexpr const T* begin() const
		{
			assert(_data);
			return std::addressof(_data[0]);
		}

		[[nodiscard]] constexpr T* end()
		{
			assert(_data);
			return std::addressof(_data[_size]);
		}

		[[nodiscard]] constexpr const T* end() const
		{
			assert(_data);
			return std::addressof(_data[_size]);
		}
	private:
		constexpr void move(vector& o)
		{
			_capacity = o._capacity;
			_size = o._size;
			_data = o._data;
			o.reset();
		}

		constexpr void reset()
		{
			_capacity = 0;
			_size = 0;
			_data = nullptr;
		}

		constexpr void destruct_range(u64 first, u64 last)
		{
			assert(destruct);
			assert(first <= _size && last <= _size && first <= last);
			if (_data)
			{
				for (; first != last; ++first)
				{
					_data[first].~T();
				}
			}
		}

		constexpr void destroy()
		{
			assert([&] {return _capacity ? _data != nullptr : _data == nullptr; }());
			clear();
			_capacity = 0;
			if (_data) free(_data);
			_data = nullptr;
		}

		u64 _capacity{ 0 };
		u64 _size{ 0 };
		T* _data{ nullptr };
	};
}