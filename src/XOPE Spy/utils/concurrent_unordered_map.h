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
	const T& operator[](K key);

	bool contains(K key);

	void clear();
private:
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
inline const T& ConcurrentUnorderedMap<K, T>::operator[](K key)
{
	if (!contains(key))
		throw std::out_of_range("[ConcurrentUnorderedMap] key does not exist");

	return _internalMap[key];
}

template<class K, class T>
inline bool ConcurrentUnorderedMap<K, T>::contains(K key)
{
	return _internalMap.contains(key);
}

template<class K, class T>
inline void ConcurrentUnorderedMap<K, T>::clear()
{
	_internalMap.clear();
}
