import React from 'react'
import { GalleryVerticalEnd } from "lucide-react"
import { RegisterForm } from '@/components/register-form'

const RegisterPage = () => {
  return (
    <div className="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <div className="flex w-full max-w-sm flex-col gap-6">
        <a
          href="https://github.com/lastnurik"
          className="flex items-center gap-2 self-center font-medium"
        >
          <div className="bg-primary text-primary-foreground flex size-6 items-center justify-center rounded-md">
            <GalleryVerticalEnd className="size-4" />
          </div>
          lastnurik.
        </a>
        <RegisterForm />
      </div>
    </div>
  )
}

export default RegisterPage
