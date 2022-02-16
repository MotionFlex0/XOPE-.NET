#include "guid.h"

Guid Guid::newGuid()
{
	GUID guid;
	CoCreateGuid(&guid);
	return { guid };
}

Guid::Guid(GUID guid) : m_guid(guid) { }

Guid::Guid(std::string guid)
{
	std::wstring wguid(guid.begin(), guid.end());
	wguid.insert(wguid.begin(), '{');
	wguid.push_back('}');

	HRESULT ret = CLSIDFromString(wguid.data(), &m_guid);
}

GUID Guid::get() const
{
	return m_guid;
}

std::string Guid::toString() const
{
	LPOLESTR pString;
	HRESULT res = StringFromCLSID(m_guid, &pString);
	
	if (res == S_OK)
	{
		std::string guidAsString;

		LPOLESTR it = pString;
		std::unique_ptr<char> mb = std::make_unique<char>(MB_CUR_MAX);
		while (*it != '\x00')
		{
			int len;
			errno_t ret = wctomb_s(&len, mb.get(), MB_CUR_MAX, *it);
			if (ret == 0 && len == 1)
				guidAsString.push_back(*mb);
			it++;
		}

		CoTaskMemFree(pString);
		return guidAsString;
	}

	return { };
}

bool Guid::operator==(const GUID& guid) const
{
	return *this == Guid(guid);
}

bool Guid::operator==(const Guid& guid) const
{
	return this->toString() == guid.toString();
}

Guid::operator GUID() const
{
	return m_guid;
}
