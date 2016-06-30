#include "Packet.h"
#include "Measure.h"
#include "Receiver.h"
#include "Protocol.h"
#include <vector>
#include <iostream>

using namespace std;
using namespace Measure;

template<class T>
void delete_pointervector(vector<T*> vec)
{
	for (auto it = vec.begin(); it != vec.end(); ++it)
		delete *it;
}

enum SerializerType : char
{
	Serializer_LSW = 0x00,
	Serializer_KHW = 0x01,
	Serializer_SHK = 0x02,
	Serializer_CPP = 0x03,
};

char* GetSerializerName(char type)
{
	switch ((SerializerType)type)
	{
	case Serializer_LSW: return "LSW";
	case Serializer_KHW: return "KHW";
	case Serializer_SHK: return "SHK";
	case Serializer_CPP: return "CPP";
	default:break;
	}
}

bool ReadConfigFile(const char* pszFilePath, vector<ConfigPacket>& configList)
{
	FILE* configFile = fopen(pszFilePath, "rb");
	while (!feof(configFile))
	{
		ConfigPacketHeader header;
		fread(&header, 1, sizeof(ConfigPacketHeader), configFile);
		if (header.prefix != ConfigPacketHeader::PREFIX) return false;

		void* config = malloc(header.payloadSize);
		fread(config, 1, header.payloadSize, configFile);

		ConfigPacket packet;
		packet.header = header;
		packet.payload = config;
		configList.push_back(packet);
	}
	fclose(configFile);

	return true;
}

void main()
{
	vector<ConfigPacket> configList;

	ReadConfigFile("D:\\Project\\TestCms1_LSW\\bin\\Debug\\Config.dat", configList);
	
	vector<IMeasure*> measures;
	for (auto it = configList.begin(); it != configList.end(); ++it)
	{
		auto packet = *it;
		if (packet.header.packetType == PacketType_MeasureConfig)
		{
			MeasureReader reader;
			measures.push_back(reader.Read(packet));
		}
		else if (packet.header.packetType == PacketType_ReceiverConfig)
		{
			switch ((ReceiverType)packet.header.subType)
			{
			case ReceiverType_Vdpm:
			{
				Receiver_VDPM* pVdpmRcv = (Receiver_VDPM*)packet.payload;
				printf("[ VDPM Receiver ]\nModuleIp:%s\n\n", pVdpmRcv->moduleIp);
				break;
			}
			case ReceiverType_File: 
			{
				Receiver_File* pFileRcv = (Receiver_File*)packet.payload;
				printf("[ File Receiver ]\nFilePath:%s Serializer:%s\n\n", pFileRcv->filePath, GetSerializerName(pFileRcv->serializerType));
				break;
			}
			case ReceiverType_Network:
			{
				Receiver_NetWork* pNetRcv = (Receiver_NetWork*)packet.payload;
				printf("[ Network Receiver ]\nServerIp:%s\nPort:%d Serializer:%s\n\n", pNetRcv->serverIp, pNetRcv->port, GetSerializerName(pNetRcv->serializerType));
				break;
			}
			}
		}
	}
	char wait;
	cin >> wait;
	delete_pointervector(measures);
}
