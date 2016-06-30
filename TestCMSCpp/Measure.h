#pragma once
#include "Packet.h"
#include <stdio.h>

namespace Measure
{
	enum MeasureType : char
	{
		MeasureType_P2P,
		MeasureType_PK,
	};

	struct PeakMeasureConfig
	{
		int MeasureId;
		int ChannelId;
	};

	struct PeakToPeakMeasureConfig
	{
		int MeasureId;
		int ChannelId;
	};

	class IMeasure
	{
		virtual float Measure(float timeData[], float fftData[], int dataCount) = 0;
	};

	struct PeakMeasure : public IMeasure
	{
		PeakMeasureConfig config;
		float Measure(float timeData[], float fftData[], int dataCount) { return 0; }

		PeakMeasure(const PeakMeasureConfig& config) { this->config = config; }
	};

	struct P2PMeasure : public IMeasure
	{
		PeakToPeakMeasureConfig config;
		float Measure(float timeData[], float fftData[], int dataCount) { return 0; }

		P2PMeasure(const PeakToPeakMeasureConfig& config) { this->config = config; }
	};

	class MeasureReader
	{
	public:
		IMeasure* Read(const ConfigPacket& packet)
		{
			switch ((MeasureType)packet.header.subType)
			{
			case MeasureType_P2P:
			{
				PeakToPeakMeasureConfig* pP2p = (PeakToPeakMeasureConfig*)packet.payload;
				printf("[ P2P Config ]\nMeasureId:%d\nChannelId:%d\n\n", pP2p->MeasureId, pP2p->ChannelId+1);
				return new P2PMeasure(*pP2p);
			}
			case MeasureType_PK:
			{
				PeakMeasureConfig* pPk = (PeakMeasureConfig*)packet.payload;
				printf("[ PK Config ]\nMeasureId:%d\nChannelId:%d\n\n", pPk->MeasureId, pPk->ChannelId+1);
				return new PeakMeasure(*pPk);
			}
			}
		}
	};
}
 