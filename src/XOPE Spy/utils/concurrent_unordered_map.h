#pragma once
#include <functional>
#include <mutex>
#include <unordered_map>

/// <summary>
/// thread-safe unordered-map. Key need to exist using 'add' before using any getter or setter operations. 
/// </summary>
/// <typeparam name="K">key type</typeparam>
/// <typeparam name="T">obj type</typeparam>
template<class K, class T>
class ConcurrentUnorderedMap
{
public:
	bool tryAdd(K key, T val);
	bool tryRemove(K key);
	bool tryGetValue(K key, const T* outputVal);

	// a thread-safe way to update T (for example if T refers to a object with multiple members)
	bool tryUpdateValueFunc(K key, std::function<void(T&)> func);

	// throws exception if key does not exist
	const T& operator[](K key) const;

	bool contains(K key) const;

	void clear();

	int count() const;

	const auto begin() const;
	const auto end() const;
private:
	//TODO: Maybe add another map with v being std::mutex to allow for per-entry locking instead of using _mutex
	std::unordered_map<K, T> _internalMap;
	std::mutex _mutex;
};

template<class K, class T>
inline bool ConcurrentUnorderedMap<K, T>::tryAdd(K key, T val)
{
	if (contains(key))
		return false;
	
	std::lock_guard<std::mutex> lock(_mutex);

	if (contains(key))
		return false;

	_internalMap.emplace(key, val);
	return true;
}

template<class K, class T>
inline bool ConcurrentUnorderedMap<K, T>::tryRemove(K key)
{
	// Could do this once, but there is no point in waiting for the lock if the key does not exist
	if (!contains(key))
		return false;

	std::lock_guard<std::mutex> lock(_mutex);

	// Make sure the the key still exists after the lock has been acquired.
	if (!contains(key))
		return false;

	_internalMap.erase(key);
	return true;
}

template<class K, class T>
inline bool ConcurrentUnorderedMap<K, T>::tryGetValue(K key, const T* outputVal)
{
	if (!contains(key) || outputVal == nullptr)
		return false;

	outputVal = _internalMap.at(key);
	return true;
}

template<class K, class T>
inline bool ConcurrentUnorderedMap<K, T>::tryUpdateValueFunc(K key, std::function<void(T&)> func)
{
	if (!contains(key))
		return false;

	std::lock_guard<std::mutex> lock(_mutex);

	if (!contains(key))
		return false;

	func(_internalMap.at(key));
	return true;
}

template<class K, class T>
inline const T& ConcurrentUnorderedMap<K, T>::operator[](K key) const
{
	if (!contains(key))
		throw std::out_of_range("[ConcurrentUnorderedMap] key does not exist");

	return _internalMap.at(key);
}

template<class K, class T>
inline bool ConcurrentUnorderedMap<K, T>::contains(K key) const
{
	return _internalMap.contains(key);
}

template<class K, class T>
inline void ConcurrentUnorderedMap<K, T>::clear()
{
	_internalMap.clear();
}

template<class K, class T>
inline int ConcurrentUnorderedMap<K, T>::count() const
{
	return _internalMap.size();
}

template<class K, class T>
inline const auto ConcurrentUnorderedMap<K, T>::begin() const
{
	return _internalMap.begin();
}

template<class K, class T>
inline const auto ConcurrentUnorderedMap<K, T>::end() const
{
	return _internalMap.end();
}
