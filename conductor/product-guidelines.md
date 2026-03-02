# Product Guidelines: Memlane

## Communication & Tone
Memlane adopts a **Professional & Technical** communication style.
- **Precision:** All logs, status updates, and dashboard notifications must be precise and provide relevant technical details (e.g., exact hash values, specific SQL error codes).
- **No Ambiguity:** Avoid vague terms like "something went wrong." Instead, provide the specific operation that failed and the underlying reason.
- **Internal Consistency:** Use consistent terminology across the C# backend and the Next.js frontend.

## Visual Aesthetic & Design
The dashboard will follow a **Corporate & Precise** design philosophy.
- **Data Density:** Layouts should prioritize information density without sacrificing clarity, using clean lines and a structured hierarchy.
- **Clarity:** Utilize a consistent color palette to represent different job statuses (e.g., Success, Warning, Failure, In Progress).
- **Responsive & Modern:** While corporate, the interface should feel modern and responsive, leveraging TypeScript for a robust user experience.

## User Experience (UX) Principles
Memlane's UX is built on three core pillars:
- **Speed & Efficiency:** Minimize the steps required to configure a new backup job. Provide sensible defaults and rapid job execution.
- **Trust & Visibility:** Users must always know the state of their data. Provide real-time status indicators and a comprehensive audit log for every action taken by the system.
- **Simplicity & Automation:** Abstract complex operations (like multi-step file synchronization and database backups) behind intuitive toggles, while allowing advanced users to fine-tune configurations.

## Error Handling & Resilience
- **Graceful Recovery:** For transient errors (e.g., network blips during a cloud upload), the system should attempt a series of retries before alerting the user.
- **Detailed Diagnostic Logs:** Every failure must be accompanied by detailed diagnostic information stored in logs and accessible via the dashboard for rapid troubleshooting.
- **Data Integrity:** Never sacrifice data integrity for speed. If a hash check fails, the operation must be halted and the user notified immediately.