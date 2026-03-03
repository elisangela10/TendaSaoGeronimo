# TendaSaoGeronimo
Api Tenda São Geronimo

## Endpoints construidos

#### User
#### Gira
#### TextoPonto
#### FileUpload

## Configuração de ambiente (obrigatória)

Para segurança, os segredos não ficam versionados no repositório.
Defina as configurações abaixo por variável de ambiente (ou Secret Manager):

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `Database__ApplyMigrationsOnStartup` (opcional, default `false`)

Exemplo (bash):

```bash
export ConnectionStrings__DefaultConnection="Host=...;Port=5432;Database=...;Username=...;Password=..."
export JwtSettings__SecretKey="uma-chave-forte-com-32+-caracteres"
export JwtSettings__Issuer="CasaDeAxeAPI"
export JwtSettings__Audience="CasaDeAxeClient"
export Database__ApplyMigrationsOnStartup="false"
```

## Robustez operacional

- Tratamento global de erros com retorno em `application/problem+json`.
- Validação de entrada com DataAnnotations e resposta padronizada com `ValidationProblemDetails`.
- Endpoint de saúde: `GET /health`.
- Verificação de conexão com banco no startup e migrations opcionais controladas por configuração.
