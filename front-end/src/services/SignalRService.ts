import * as signalR from '@microsoft/signalr';
import { JobStatusUpdate } from '@/models/Job';

const HUB_URL = process.env.NEXT_PUBLIC_HUB_URL;

export class SignalRService {
    private connection: signalR.HubConnection | null = null;

    constructor(private onStatusUpdate: (update: JobStatusUpdate) => void) {}

    public async start() {
        if (this.connection) return;

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${HUB_URL}/jobs`)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.on("ReceiveGlobalStatusUpdate", (update: JobStatusUpdate) => {
            this.onStatusUpdate(update);
        });

        try {
            await this.connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            setTimeout(() => this.start(), 5000);
        }
    }

    public async stop() {
        if (this.connection) {
            await this.connection.stop();
            this.connection = null;
        }
    }

    public async joinJobGroup(jobId: number) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("JoinJobGroup", jobId);
        }
    }
}
