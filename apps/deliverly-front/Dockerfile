FROM node:20-slim
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
EXPOSE 5173
# O Vite precisa do flag --host para ser acessível fora do container
CMD ["npm", "run", "dev", "--", "--host"]