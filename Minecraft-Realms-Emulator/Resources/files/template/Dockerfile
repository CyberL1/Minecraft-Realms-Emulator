FROM eclipse-temurin:21-jre-alpine
COPY . .

RUN apk upgrade && \
    apk add openssl curl jq && \
    wget -O rcon-cli.tar.gz https://github.com/itzg/rcon-cli/releases/download/1.7.3/rcon-cli_1.7.3_linux_amd64.tar.gz && \
    tar xzf rcon-cli.tar.gz && \
    rm rcon-cli.tar.gz && \
    mv rcon-cli /bin

WORKDIR /mc
ENTRYPOINT ["/entrypoint"]
