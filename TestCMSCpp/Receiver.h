#pragma once
#define MAX_IP_LEN 20
#define MAX_SERIALIZER_LEN 4
#define MAX_STR_LEN 200
#define MAX_PORT_LEN 8

enum ReceiverType
{
	ReceiverType_Vdpm,
	ReceiverType_File,
	ReceiverType_Network
};

struct Receiver_File
{
	char serializerType;
	char filePath[MAX_STR_LEN +3];
};

struct Receiver_VDPM
{
	char moduleIp[MAX_IP_LEN];
};

struct Receiver_NetWork
{
	char serializerType;
	char reserved[3];
	int port;
	char serverIp[MAX_IP_LEN];
};