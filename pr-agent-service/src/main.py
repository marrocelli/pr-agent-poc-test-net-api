
from fastapi import FastAPI, HTTPException, Depends, BackgroundTasks
from fastapi.responses import JSONResponse
import uvicorn
from .agent import Agent
from .config.settings import settings

app = FastAPI(title="PR Agent Service")

@app.get("/health")
async def health_check():
    return {"status": "healthy", "version": "1.0.0"}

@app.post("/agent/pr-review")
async def handle_pr_review(
    command: dict,
    background_tasks: BackgroundTasks
):
    """
    Receives PR command from C# webhook.
    
    Example payload:
    {
        "repo": "username/test-pr-agent-net",
        "prNumber": 1,
        "action": "opened"
    }
    """
    try:
        agent = Agent(settings)
        result = await agent.run(command)
        return result
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    uvicorn.run(
        "src.main:app",
        host="0.0.0.0",
        port=8000,
        reload=True
    )
