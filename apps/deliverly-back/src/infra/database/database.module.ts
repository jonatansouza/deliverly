import { Global, Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import Nano from 'nano';

export const COUCHDB_CONNECTION = 'COUCHDB_CONNECTION';

@Global()
@Module({
  imports: [ConfigModule],
  providers: [
    {
      provide: COUCHDB_CONNECTION,
      inject: [ConfigService],
      useFactory: async (configService: ConfigService) => {
        const url = configService.get<string>('COUCHDB_URL')!;
        const user = configService.get<string>('COUCHDB_USER') ?? '';
        const pass = configService.get<string>('COUCHDB_PASSWORD') ?? '';

        const nano = Nano(url);

        await nano.auth(user, pass);

        try {
          await nano.db.create('tickets');
        } catch (err) {
          if (err.statusCode === 412) {
            console.log('Database "tickets" already exists');
          } else {
            throw err;
          }
        }

        return nano;
      },
    },
  ],
  exports: [COUCHDB_CONNECTION],
})
export class DatabaseModule {}
