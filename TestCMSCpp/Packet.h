#pragma once
#include "Protocol.h"

#define ASYNCDATACOUNT 8192
enum PacketType : char
{
	PacketType_Invalid = 0,
	PacketType_WaveData,
	PacketType_ReceiverConfig,
	PacketType_MeasureConfig,
};

struct WaveData
{
	enum { AsyncMaxSize = ASYNCDATACOUNT };
	int asyncDataCount;
	int channelId;
	double dateTime;
	float asyncData[AsyncMaxSize];
};

struct ConfigPacketHeader
{
	enum { PREFIX = 0xab };

	unsigned char prefix=0;
	PacketType packetType;
	unsigned char subType;
	unsigned char reserved[1];
	int payloadSize;
};

struct ConfigPacket
{
	ConfigPacketHeader header;
	void* payload;
};

