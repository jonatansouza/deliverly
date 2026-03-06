import { INestApplication, Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { ClientsModule, MicroserviceOptions, Transport } from '@nestjs/microservices';

@Module({
  imports: [
    ConfigModule,
    ClientsModule.registerAsync([
      {
        name: 'TICKET_SERVICE',
        imports: [ConfigModule],
        inject: [ConfigService],
        useFactory: (configService: ConfigService) => {
          const broker = configService.get<string>('KAFKA_BROKER')!;
          const groupId = configService.get<string>('KAFKA_GROUP_ID')!;
          const config = {
            transport: Transport.KAFKA,
            options: {
              client: {
                brokers: [broker],
              },
              consumer: {
                groupId,
              },
            },
          };
          return config as any;
        },
      },
    ]),
  ],
  exports: [ClientsModule],
})
export class KafkaModule {}

export const kafkaConsumerConfig = (app: INestApplication, configService: ConfigService) => {
  const broker = configService.get<string>('KAFKA_BROKER')!;
  const groupId = configService.get<string>('KAFKA_GROUP_ID')!;
  app.connectMicroservice<MicroserviceOptions>({
    transport: Transport.KAFKA,
    options: {
      client: { brokers: [broker] },
      consumer: { groupId },
    },
  });
};
