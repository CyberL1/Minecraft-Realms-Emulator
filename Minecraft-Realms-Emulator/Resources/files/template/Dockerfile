FROM eclipse-temurin:21-jre-alpine

COPY . .

RUN apk upgrade
RUN apk add openssl curl jq

RUN wget -O rcon-cli.tar.gz https://github.com/itzg/rcon-cli/releases/download/1.6.7/rcon-cli_1.6.7_linux_amd64.tar.gz
RUN tar xzf rcon-cli.tar.gz

RUN rm rcon-cli.tar.gz
RUN mv rcon-cli /bin/rcon-cli

WORKDIR mc
EXPOSE 25565

ENTRYPOINT ["/entrypoint"]