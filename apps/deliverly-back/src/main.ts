import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  app.enableCors();
  app.setGlobalPrefix('api');

  //const configService = app.get(ConfigService);
  //kafkaConsumerConfig(app, configService);

  //await app.startAllMicroservices();
  await app.listen(process.env.PORT ?? 3000);
}
bootstrap();
