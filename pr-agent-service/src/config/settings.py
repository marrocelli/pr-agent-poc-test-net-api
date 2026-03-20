
from pydantic_settings import BaseSettings
from typing import List

class Settings(BaseSettings):
    github_token: str
    github_repo: str
    
    azure_openai_endpoint: str
    azure_openai_api_key: str
    azure_openai_deployment: str = "gpt-4o"
    
    azure_subscription_id: str
    azure_tenant_id: str
    azure_client_id: str
    azure_client_secret: str
    
    agent_max_steps: int = 10
    agent_timeout_seconds: int = 300
    
    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"

settings = Settings()
